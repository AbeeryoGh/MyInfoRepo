using EngineCoreProject.DTOs.MeetingDto;
using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.Models;


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;

using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Data.SqlClient;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Utility;
using System.Net.Http;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using EngineCoreProject.IRepository.IFilesUploader;
using Microsoft.Extensions.Configuration;
using EngineCoreProject.IRepository.IQueueRepository;

namespace EngineCoreProject.Services.Meetings
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IUserRepository _iUserRepository;
        private readonly ILogger<MeetingRepository> _iLogger;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly IQueueRepository _IQueueRepository;
        ValidatorException _exception;
        private readonly IConfiguration _IConfiguration;

        public MeetingRepository(EngineCoreDBContext EngineCoreDBContext, IUserRepository iUserRepository,
                                 IFilesUploaderRepositiory iFilesUploaderRepository, IQueueRepository iQueueRepository,
                                 IConfiguration iConfiguration, ILogger<MeetingRepository> iLogger)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iUserRepository = iUserRepository;
            _exception = new ValidatorException();
            _iLogger = iLogger;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IConfiguration = iConfiguration;
            _IQueueRepository = iQueueRepository;
        }

        public MeetingRepository(EngineCoreDBContext EngineCoreDBContext)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
        }

        public async Task<Meeting> AddMeeting(MeetingPostDto meetingDto, int userId, string lang)
        {
            ValidateMeeting(meetingDto, lang);
            Meeting meeting = meetingDto.GetEntity();
            meeting.UserId = userId;
            // generate new meeting id.
            int meetingId = GetNewValueByMeetingSec();
            int sumDigits = 0;
            int temp = meetingId;
            while (temp > 0)
            {
                sumDigits += temp % 10;
                temp /= 10;
            }
            sumDigits %= 100;
            meeting.MeetingId = sumDigits < 10 ? meetingId.ToString() + "0" + sumDigits.ToString() : meetingId.ToString() + sumDigits.ToString();

            await _EngineCoreDBContext.Meeting.AddAsync(meeting);
            await _EngineCoreDBContext.SaveChangesAsync();

            return meeting;
        }

        public async Task<Meeting> UpdateMeeting(int rowId, MeetingPostDto meetingDto, int userId, string lang)
        {
            ValidateMeeting(meetingDto, lang);
            Meeting originalMeeting = await _EngineCoreDBContext.Meeting.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalMeeting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroResult") + " " + rowId);
                throw _exception;
            }

            originalMeeting.Topic = meetingDto.Topic;
            originalMeeting.StartDate = meetingDto.StartDate;
            originalMeeting.EndDate = meetingDto.EndDate;
            originalMeeting.TimeZone = meetingDto.TimeZone;
            originalMeeting.Description = meetingDto.Description;
            originalMeeting.Password = meetingDto.Password;
            originalMeeting.Status = meetingDto.Status;
            originalMeeting.OrderNo = meetingDto.OrderNo;

            originalMeeting.UserId = userId;
            _EngineCoreDBContext.Meeting.Update(originalMeeting);
            await _EngineCoreDBContext.SaveChangesAsync();

            return originalMeeting;
        }

        public async Task<List<MeetingGetDto>> GetMeetings(int userId, string lang)
        {
            // TODO: get authorized user meeting.
            var isExist = _EngineCoreDBContext.User.Where(x => x.Id == userId).FirstOrDefault();
            if (isExist == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroResult") + " " + userId);
                throw _exception;
            }

            List<Meeting> createdMeeting = new List<Meeting>();

            createdMeeting = await _EngineCoreDBContext.Meeting.ToListAsync();

            List<MeetingGetDto> result = new List<MeetingGetDto>();
            foreach (var meeting in createdMeeting)
            {
                result.Add(MeetingGetDto.GetDTO(meeting));
            }
            return result;
        }

        public async Task<MeetingGetDto> GetMeetingById(int id, string lang)
        {
            Meeting createdMeeting = new Meeting();
            createdMeeting = await _EngineCoreDBContext.Meeting.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroResult") + " " + id);
                throw _exception;
            }
            return MeetingGetDto.GetDTO(createdMeeting);
        }

        public async Task<MeetingGetDto> GetMeetingByMeetingId(string meetingId, string lang)
        {
            Meeting createdMeeting = new Meeting();
            createdMeeting = await _EngineCoreDBContext.Meeting.Where(d => d.MeetingId == meetingId).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroResult"));
                throw _exception;
            }
            return MeetingGetDto.GetDTO(createdMeeting);
        }

        public async Task<List<MeetingGetDto>> GetMeetingByOrderNo(string orderNo)
        {
            List<MeetingGetDto> res = new List<MeetingGetDto>();
            if (orderNo == null)
            {
                return res;
            }

            var createdMeeting = await _EngineCoreDBContext.Meeting.Where(d => d.OrderNo == orderNo).ToListAsync();
            if (createdMeeting == null)
            {
                return res;
            }

            foreach (var meet in createdMeeting)
            {
                res.Add(MeetingGetDto.GetDTO(meet));
            }
            return res;
        }

        public async Task<MeetingGetDto> GetMeetingByMeetingIdAndPassword(string meetingId, string password, string lang)
        {
            Meeting createdMeeting = new Meeting();
            createdMeeting = await _EngineCoreDBContext.Meeting.Where(d => d.Password == password && d.MeetingId == meetingId).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroResult"));
                throw _exception;
            }
            return MeetingGetDto.GetDTO(createdMeeting);
        }

        public async Task<IsAttended> IsAttendedByAppNo(int orderNo)
        {
            IsAttended isAttended = new IsAttended
            {
                IsLate = false,
                IsOnline = false,
                LastLogIn = null
            };

            var meets = await _EngineCoreDBContext.Meeting.Include(x => x.MeetingLogging).Where(x => x.OrderNo == orderNo.ToString()).OrderByDescending(x => x.Id).ToListAsync();

            if (meets.Count > 0)
            {
                var meet = meets[0];
                var meetLooging = meet.MeetingLogging.Where(x => x.IsModerator == false && x.LoginDate.AddSeconds(122) >= DateTime.Now).OrderBy(x => x.FirstLogin).ToList();
                if (meetLooging.Count() > 0)
                {
                    isAttended.IsOnline = true;
                    isAttended.LastLogIn = meetLooging[0].FirstLogin;
                    if (meet.MeetingLogging.Any(x => x.IsModerator == false && x.FirstLogin.AddMinutes(15) < DateTime.Now))
                    {
                        isAttended.IsLate = true;
                    }
                }
            }
            return isAttended;
        }

        public async Task<bool> LogInToMeeting(string meetingNo)
        {
            try
            {
                var meet = await _EngineCoreDBContext.Meeting.Include(x => x.MeetingLogging).Where(x => x.MeetingId == meetingNo).FirstOrDefaultAsync();
                if (meet != null)
                {
                    var userId = _iUserRepository.GetUserID();
                    var oldLogging = meet.MeetingLogging.Where(x => x.UserId == userId).FirstOrDefault();
                    if (oldLogging == null)
                    {
                        MeetingLogging meetLog = new MeetingLogging
                        {
                            IsModerator = false,
                            UserId = userId,
                            LoginDate = DateTime.Now,
                            MeetingId = meet.Id,
                            FirstLogin = DateTime.Now
                        };

                        await _EngineCoreDBContext.MeetingLogging.AddAsync(meetLog);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                    else
                    {
                        oldLogging.LoginDate = DateTime.Now;
                        _EngineCoreDBContext.MeetingLogging.Update(oldLogging);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangeMeetingStatusBackToPendingByAppId(string applicationId, string lang)
        {
            var originalMeetingList = await _EngineCoreDBContext.Meeting.Where(a => a.OrderNo == applicationId).ToListAsync();
            foreach (var originalMeeting in originalMeetingList)
            {
                if (originalMeeting == null || originalMeeting.Status == (int)Constants.MEETING_STATUS.FINISHED || originalMeeting.Status == (int)Constants.MEETING_STATUS.PENDING)
                {
                    continue;
                }

                originalMeeting.LastUpdatedBy = _iUserRepository.GetUserID();
                originalMeeting.LastUpdatedDate = DateTime.Now;
                originalMeeting.Status = (int)Constants.MEETING_STATUS.PENDING;
                _EngineCoreDBContext.Meeting.Update(originalMeeting);
                await _EngineCoreDBContext.SaveChangesAsync();

                // TODO later from lilac.
                var deleteLogging = await _EngineCoreDBContext.MeetingLogging.Where(a => a.MeetingId == originalMeeting.Id).ToListAsync();
                _EngineCoreDBContext.MeetingLogging.RemoveRange(deleteLogging);
                await _EngineCoreDBContext.SaveChangesAsync();
            }
            return true;
        }


        public async Task<int> SetMeetingStatus(string meetingId, Constants.MEETING_STATUS newStatus, bool changeAppointment, string lang)
        {
            Meeting originalMeeting = await _EngineCoreDBContext.Meeting.Where(a => a.MeetingId == meetingId).FirstOrDefaultAsync();

            if (originalMeeting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroResult" + " " + meetingId));
                throw _exception;
            }

            originalMeeting.Status = newStatus switch
            {
                Constants.MEETING_STATUS.FINISHED => (int)Constants.MEETING_STATUS.FINISHED,
                Constants.MEETING_STATUS.PENDING => (int)Constants.MEETING_STATUS.PENDING,
                Constants.MEETING_STATUS.STARTED => (int)Constants.MEETING_STATUS.STARTED,
                _ => throw _exception,
            };

            if (changeAppointment)
            {
                originalMeeting.StartDate = DateTime.Now.AddMinutes(-10);
                originalMeeting.EndDate = DateTime.Now;
            }

            originalMeeting.LastUpdatedBy = _iUserRepository.GetUserID();
            originalMeeting.LastUpdatedDate = DateTime.Now;
            _EngineCoreDBContext.Meeting.Update(originalMeeting);
            await _EngineCoreDBContext.SaveChangesAsync();
            return originalMeeting.Id;
        }

        public async Task<bool> MeetingHasPassword(string meetingId)
        {
            return await _EngineCoreDBContext.Meeting.AnyAsync(x => x.MeetingId == meetingId && x.PasswordReq == true);
        }

        public async Task<bool> IfExistMeeting(string meetingId)
        {
            return await _EngineCoreDBContext.Meeting.AnyAsync(x => x.MeetingId == meetingId);
        }

        public async Task<object> MeetingJWT(string meetingId, int? userId, string userName, string lang, string meetingPassword)
        {
            var createdMeeting = await _EngineCoreDBContext.Meeting.Where(d => d.MeetingId == meetingId).FirstOrDefaultAsync();
            if (createdMeeting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + meetingId);
                throw _exception;
            }

            if (createdMeeting.Status == (int)Constants.MEETING_STATUS.FINISHED)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "meetingFinished") + meetingId);
                _iLogger.LogInformation(string.Format("try an access to finish meeting, {0}", meetingId));
                throw _exception;
            }


            bool isModerator = false;
            // context.user.id and contex.group should be fresh-generated UUID values(Do not put actual user ids here).
            string autoUserId = Guid.NewGuid().ToString();

            string confUrlPrefix = _IConfiguration["CONF_URL_PREFIX"];
            if (confUrlPrefix == null || confUrlPrefix.Length == 0)
            {
                _exception.AttributeMessages.Add("Missing configuration for lilac CONF_URL_PREFIX ");
                throw _exception;
            }


            string sub = confUrlPrefix;
            if (confUrlPrefix.StartsWith("https://"))
            {
                sub = confUrlPrefix.Substring("https://".Length);
            }

            if (confUrlPrefix.StartsWith("http://"))
            {
                sub = confUrlPrefix.Substring("http://".Length);
            }

            UserStruct userInfo = new UserStruct()
            {
                id = autoUserId,
                // default user image.
                avatar = "data:image/png;base64,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",
                name = userName,
                email = "not available"
            };

            if (userId == null)
            {
                if (createdMeeting.PasswordReq == true)
                {
                    if (createdMeeting.Password != meetingPassword)
                    {
                        _exception.AttributeMessages.Add(Constants.getMessage(lang, "errorAuthentication") + meetingPassword);
                        throw _exception;
                    }
                }
            }
            else
            {
                var user = _EngineCoreDBContext.User.Where(x => x.Id == userId).SingleOrDefault();
                if (user == null)
                {
                    _exception.AttributeMessages.Add(string.Format("This user is not authorized \'{0}\'.", userId));
                    throw _exception;
                }
                userInfo.name = user.FullName;
                userInfo.email = user.Email;

                byte[] bytes = new byte[16];
                BitConverter.GetBytes(user.Id).CopyTo(bytes, 0);
                EncryptGUID encryptGUID = new EncryptGUID(bytes);
                byte[] guid = new byte[16];
                var encruptedGuid = new Guid(encryptGUID.encryptUID(guid));
                userInfo.id = encruptedGuid.ToString();

                if (_iUserRepository.IsAdmin() || _iUserRepository.IsEmployee())
                {
                    isModerator = true;
                }
            }

            try
            {
                // get user image.
                string getUserImageURL = _IConfiguration["GetUserImage"];
                if (userId != null && _IFilesUploaderRepository.FileExist("User_images", userId.ToString() + ".jpg"))
                {

                    userInfo.avatar = getUserImageURL + userId.ToString() + ".jpg";
                }
                else
                {
                    if (_IFilesUploaderRepository.FileExist("User_images", "default.jpg"))
                    {
                        userInfo.avatar = getUserImageURL + "default.jpg";
                    }
                }
            }
            catch
            {

            }


            // generate JWT.
            IdentityOptions identityOptions = new IdentityOptions();
            string groupId = Guid.NewGuid().ToString();
            DateTime expirationDate = DateTime.Now.AddDays(1);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = expirationDate - origin;
            double doublec = Math.Floor(diff.TotalSeconds);


            Context contxt = new Context()
            {
                user = userInfo,
                group = groupId
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", "enotaryWeb" },
                { "aud", "enotaryWeb" },
                { "sub", sub },
                {  "room", meetingId},
                { "moderator", isModerator },
                { "context", contxt },
                { "exp", doublec }
            };


            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, Constants.meetingBase32Ssecret);

            // Console.WriteLine(token);

            // TODO check if correct only one day.
            //var hours = (createdMeeting.EndDate - DateTime.Now).TotalHours;
            //var jwtToken = new JwtSecurityToken(token, expires: DateTime.Now.AddHours(hours));

            // start the meeting.
            if (isModerator)
            {
                await SetMeetingStatus(meetingId, Constants.MEETING_STATUS.STARTED, true, "en");
                await _IQueueRepository.ChangeTicketsStatusToInProgressByProcessNo(_iUserRepository.GetUserID(), createdMeeting.OrderNo);

                // reset the timer for participants logging.
                try
                {
                    var allUserlogging = await _EngineCoreDBContext.MeetingLogging.Where(x => x.MeetingId == createdMeeting.Id).ToListAsync();
                    if (allUserlogging != null)
                    {
                        foreach (var userlogging in allUserlogging)
                        {
                            if (!userlogging.IsModerator)
                            {
                                UserLog myDeserializedObj = new UserLog();
                                if (userlogging.PreviousLoginList != null)
                                {
                                    myDeserializedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLog>(userlogging.PreviousLoginList);
                                }

                                LogEntry entry = new LogEntry
                                {
                                    Start = userlogging.FirstLogin,
                                    End = userlogging.LoginDate
                                };
                                myDeserializedObj.UserLogs.Add(entry);

                                userlogging.PreviousLoginList = JsonConvert.SerializeObject(myDeserializedObj, Formatting.Indented);
                                userlogging.FirstLogin = DateTime.Now;
                                userlogging.LoginDate = DateTime.Now;

                                _EngineCoreDBContext.MeetingLogging.Update(userlogging);
                                await _EngineCoreDBContext.SaveChangesAsync();
                            }

                        }
                    }
                }
                catch
                {
                    _iLogger.LogInformation(string.Format("Failed in reseting the timer for participants logging to the meeting {0}", meetingId));
                }

            }

            if (userId != null)
            {
                try
                {
                    var loggingUser = await _EngineCoreDBContext.MeetingLogging.Where(x => x.MeetingId == createdMeeting.Id && x.UserId == userId).FirstOrDefaultAsync();
                    if (loggingUser != null)
                    {                   
                        if (loggingUser.LoginDate.AddMinutes(5) < DateTime.Now)
                        {
                            // interruption attending.
                            UserLog myDeserializedObj = new UserLog();
                            if (loggingUser.PreviousLoginList != null)
                            {
                                myDeserializedObj = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLog>(loggingUser.PreviousLoginList);
                            }
                          
                            LogEntry entry = new LogEntry
                            {
                                Start = loggingUser.FirstLogin,
                                End = loggingUser.LoginDate
                            };
                            myDeserializedObj.UserLogs.Add(entry);

                            loggingUser.PreviousLoginList = JsonConvert.SerializeObject(myDeserializedObj, Formatting.Indented);
                            loggingUser.FirstLogin = DateTime.Now;
                        }

                        loggingUser.LoginDate = DateTime.Now;
                        _EngineCoreDBContext.MeetingLogging.Update(loggingUser);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                    else
                    {
                        MeetingLogging meetLog = new MeetingLogging
                        {
                            IsModerator = isModerator,
                            LoginDate = DateTime.Now,
                            MeetingId = createdMeeting.Id,
                            FirstLogin = DateTime.Now,
                            UserId = userId
                        };

                        await _EngineCoreDBContext.MeetingLogging.AddAsync(meetLog);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    var error = " error is " + ex.Message;
                    if (ex.InnerException != null && ex.InnerException.Message != null)
                    {
                        error += ex.InnerException.Message;
                    }
                    _iLogger.LogInformation(string.Format("Failed in writing login record to the meeting {0}", meetingId) + error);
                }
            }
            return new { token, confUrlPrefix };
        }


        public async Task GetMeetingLogger()
        {
            try
            {
                var allMeet = await _EngineCoreDBContext.Meeting.Where(x => x.Status == (int)Constants.MEETING_STATUS.FINISHED && x.MeetingLog == null).ToListAsync();

                if (allMeet == null || allMeet.Count == 0)
                {
                    return;
                }

                HttpClientHandler clientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };

                string confUrlPrefix = _IConfiguration["CONF_URL_PREFIX"];
                if (confUrlPrefix == null || confUrlPrefix.Length == 0)
                {
                    throw _exception;
                }

                string confUrlPrefixLog = _IConfiguration["CONF_URL_PREFIX_LOG"];
                if (confUrlPrefixLog == null || confUrlPrefixLog.Length == 0)
                {
                    throw _exception;
                }

                string confUrlAPIGetLog = _IConfiguration["CONF_URL_API_GET_LOG"];
                if (confUrlAPIGetLog == null || confUrlAPIGetLog.Length == 0)
                {
                    throw _exception;
                }

                HttpClient client = new HttpClient(clientHandler);
                client.DefaultRequestHeaders.Host = confUrlPrefixLog;

                int failed = 0;
                int success = 0;
                foreach (var meet in allMeet)
                {
                    var response = await client.GetAsync(String.Format(confUrlAPIGetLog, meet));

                    // ensure the request was a success
                    if (!response.IsSuccessStatusCode)
                    {
                        meet.MeetingLog = "failed in getting log";
                        failed++;
                    }
                    else
                    {
                        meet.MeetingLog = await response.Content.ReadAsStringAsync();
                        success++;
                    }
                    _EngineCoreDBContext.Meeting.Update(meet);
                    await _EngineCoreDBContext.SaveChangesAsync();
                }

               _iLogger.LogInformation(" GetMeetingLogger success is : " + success.ToString() + " failed records count is : " + failed.ToString());
            }

            catch (Exception ex)
            {
                _iLogger.LogInformation("error in GetMeetingLogger : " + ex.Message.ToString());
                return;
            }

            /* TODO load the string logger to JSON Object to trace users.
             var stream2 = await response.Content.ReadAsStringAsync();
             var content = JsonConvert.DeserializeObject<ConferenceLogger>(stream2);
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(@"c:\movie.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, content);
            }
            */
        }


        private void ValidateMeeting(MeetingPostDto meetingPostDto, string lang)
        {
            if (meetingPostDto.EndDate < meetingPostDto.StartDate)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " " + meetingPostDto.EndDate);
                throw _exception;
            }

            int status = (int)meetingPostDto.Status;
            if (!Enum.IsDefined(typeof(Constants.MEETING_STATUS), status))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " " + meetingPostDto.Status);
                throw _exception;
            }
        }

        private int GetNewValueByMeetingSec()
        {
            var p = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for SequenceForMeetingId", p);
            int sequenceNum = (int)p.Value;
            return sequenceNum;
        }

        public class ConferenceDetailsLogger
        {
            public string id { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public string confid { get; set; }
            public string userid { get; set; }
            public string info { get; set; }
        }

        public class ConferenceLogger
        {
            public string status { get; set; }
            public List<ConferenceDetailsLogger> data { get; set; }
        }
    }
}



public class Context
{
    public Context()
    {

    }
    public Context(UserStruct _user, string _group)
    {
        user = _user;
        group = _group;
    }

    public UserStruct user { get; set; }     // user restrict to be in small letter user accord to conference meeting.
    public string group { get; set; }        // group restrict to be in small letter group accord to conference meeting.
}

public struct UserStruct
{
    public string id { get; set; }    // id restrict to be in small letter id accord to conference meeting.
    public string avatar { get; set; }   // avatar restrict to be in small letter name accord to conference meeting.
    public string name { get; set; }   // name restrict to be in small letter name accord to conference meeting.
    public string email { get; set; }   // email restrict to be in small letter name accord to conference meeting.
}