
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using EngineCoreProject.Services.Job;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EngineCoreProject.DTOs.SMSDto;
using EngineCoreProject.DTOs.ChannelDto;
using System.Threading;

using OtpNet;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using EngineCoreProject.DTOs.JWTDto;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.Configuration;
using EngineCoreProject.Services.ApplicationSet;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.IRepository.IGeneralSetting;

namespace EngineCoreProject.Services.NotificationService
{



    public class UserInfo
    {
        public int UserId { get; set; }
        public List<UserInfoDetails> Adresses { get; set; }
        public string UserName { get; set; }
    }

    public class UserInfoDetails
    {
        public int ChannelNo { get; set; }
        public string Address { get; set; }
    }

    public class SendNotificationRepository : ISendNotificationRepository
    {
        private readonly IOptions<ChannelMailFirstSetting> _mailSetting;
        private readonly IOptions<ChannelSMSSetting> _smsSetting;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly INotificationLogRepository _iNotificationLogRepository;
        private readonly ILogger<ApplicationRepositiory> _loggerForApplication;
        private readonly ILogger<SendNotificationRepository> _logger;
        private readonly IGeneralRepository _generalRepository;
        private readonly jwt _jwt;
        private readonly IConfiguration _configuration;
        private readonly IApplicationPartyRepository _iApplicationPartyRepository;
        private readonly ITransactionRepository _iTransactionRepository;
        private readonly IUserRepository _iUserRepository;
        private readonly IWorkingTimeRepository _iWorkingTimeRepository;
        ValidatorException _exception;

        public SendNotificationRepository(EngineCoreDBContext EngineCoreDBContext, IOptions<jwt> jwt, IOptions<ChannelMailFirstSetting> mailSettings, IOptions<ChannelSMSSetting> smsSettings, INotificationLogRepository iNotificationLogRepository,
            ILogger<SendNotificationRepository> logger, IGeneralRepository generalRepository, IConfiguration configuration,
            ILogger<ApplicationRepositiory> loggerForApplication,
            IApplicationPartyRepository iApplicationPartyRepository, IWorkingTimeRepository iWorkingTimeRepository,
            ITransactionRepository iTransactionRepository,
            IUserRepository iUserRepository)
        {
            _mailSetting = mailSettings;
            _smsSetting = smsSettings;
            _EngineCoreDBContext = EngineCoreDBContext;
            _iNotificationLogRepository = iNotificationLogRepository;
            _logger = logger;
            _generalRepository = generalRepository;
            _jwt = jwt.Value;
            _configuration = configuration;
            _iApplicationPartyRepository = iApplicationPartyRepository;
            _iTransactionRepository = iTransactionRepository;
            _iUserRepository = iUserRepository;
            _iWorkingTimeRepository = iWorkingTimeRepository;
            _loggerForApplication = loggerForApplication;
            _exception = new ValidatorException();
        }

        public async Task DoSend(List<NotificationLogPostDto> notifications, bool sendImmediately, bool addOrUpdateNotificationslog)
        {
            int mailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int internalChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            List<NotificationLogPostDto> notifyByEmail = new List<NotificationLogPostDto>();
            List<NotificationLogPostDto> notifyBySMS = new List<NotificationLogPostDto>();
            List<NotificationLogPostDto> notifyByInternal = new List<NotificationLogPostDto>();

            notifications.RemoveAll(x => x.ToAddress == null || x.ToAddress.Contains(Constants.INVALID_EMAIL_SUFFIX));


            notifyByEmail.AddRange(notifications.Where(x => x.NotificationChannelId == mailChannel));
            notifyBySMS.AddRange(notifications.Where(x => x.NotificationChannelId == smsChannel));
            notifyByInternal.AddRange(notifications.Where(x => x.NotificationChannelId == internalChannel));

            ControlNotification Con = new ControlNotification();
            if (notifyByEmail.Count > 0)
            {
                INotificationObserver objEmail = new MailNotification(notifyByEmail, _mailSetting, _EngineCoreDBContext);
                Con.AddService(objEmail);
            }

            if (notifyBySMS.Count > 0)
            {
                INotificationObserver objSMS = new SMSNotification(notifyBySMS, _EngineCoreDBContext, _generalRepository, _smsSetting);
                Con.AddService(objSMS);
            }

            if (notifyByInternal.Count > 0)
            {
                INotificationObserver objInternal = new InternalNotification(notifyByInternal, _EngineCoreDBContext);
                Con.AddService(objInternal);
            }

            var res = Con.ExecuteNotifier(sendImmediately);

            try
            {
                if (addOrUpdateNotificationslog)
                {
                    await _iNotificationLogRepository.AddNotificationsLog(res);
                }
                else
                {
                    await _iNotificationLogRepository.UpdateNotificationsLog(res);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                var xx = e.InnerException.Message;
                Console.WriteLine(e.InnerException.Message);
            }
        }

        public async Task ReSend()
        {
            int internalChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            var failedNotifications = await _EngineCoreDBContext.NotificationLog.Where(x => x.IsSent != (byte)Constants.NOTIFICATION_STATUS.SENT && x.SentCount < Constants.MAX_NOTIFY_SEND_ATTEMPTS && x.NotificationChannelId != internalChannel).ToListAsync();

            if (failedNotifications.Count == 0)
            {
                return;
            }

            List<NotificationLogPostDto> notificationsDto = new List<NotificationLogPostDto>();
            foreach (var fail in failedNotifications)
            {
                notificationsDto.Add(NotificationLogPostDto.GetDto(fail));
            }

            _logger.LogInformation("call resend for " + notificationsDto.Count + " notifications " + $"{DateTime.Now:hh:mm:ss} ============");
            await DoSend(notificationsDto, true, false);
        }

        private async Task<bool> SendOTP(int userId, List<string> phones, List<string> emails, int appId, string lang)
        {
            try
            {
                int otpPeriodInMinutes = Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING;

                if (_configuration["OtpPeriodInMinutes"] == null)
                {
                    _logger.LogInformation("Warning!!! OtpPeriodInMinutes is missing");
                }
                else
                {
                    bool success = int.TryParse(_configuration["OtpPeriodInMinutes"], out int settingPeriod);
                    if (!success || settingPeriod < 1)
                    {
                        _logger.LogInformation("Warning OtpPeriodInMinutes is invalid number or < 1 minute");
                    }
                    else
                    {
                        otpPeriodInMinutes = settingPeriod;
                    }
                }


                var totp = new Totp(Base32Encoding.ToBytes(Constants.otpBase32Secret));
                var code = totp.ComputeTotp();

                var oldOtp = await _EngineCoreDBContext.OtpLog.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                if (oldOtp != null)
                {
                    if (oldOtp.GeneratedDate.AddMinutes(otpPeriodInMinutes) > DateTime.Now)
                    {
                        code = oldOtp.OtpCode;
                        oldOtp.GeneratedDate = DateTime.Now;
                        _EngineCoreDBContext.OtpLog.Update(oldOtp);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                    else
                    {
                        oldOtp.OtpCode = code;
                        oldOtp.GeneratedDate = DateTime.Now;
                        _EngineCoreDBContext.OtpLog.Update(oldOtp);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                }
                else
                {
                    OtpLog otp = new OtpLog
                    {
                        GeneratedDate = DateTime.Now,
                        OtpCode = code,
                        UserId = userId,
                    };
                    await _EngineCoreDBContext.OtpLog.AddAsync(otp);
                    await _EngineCoreDBContext.SaveChangesAsync();
                }

                List<NotificationLogPostDto> notificationsDto = new List<NotificationLogPostDto>();

                var defLang = "en";
                if (lang != null)
                {
                    defLang = lang;
                }

                var title = (defLang.Trim().ToLower() == "ar") ? Constants.OTP_TITLE_AR : Constants.OTP_TITLE_EN;
                var bodyD = (defLang.Trim().ToLower() == "ar") ? Constants.OTP_BODY_AR : Constants.OTP_BODY_EN;

                var user = await _EngineCoreDBContext.User.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user != null)
                {
                    bodyD += user.FullName + "  ";
                }

                byte[] bytes = Encoding.Default.GetBytes(bodyD);
                var body = Encoding.UTF8.GetString(bytes);

                if (phones != null)
                {
                    int smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
                    foreach (var phone in phones)
                    {
                        NotificationLogPostDto notifyBySMS = new NotificationLogPostDto()
                        {
                            NotificationChannelId = smsChannel,
                            UserId = userId,
                            Lang = defLang.Trim().ToLower(),
                            NotificationTitle = title,
                            NotificationBody = body + code,
                            ToAddress = phone,
                            ApplicationId = appId
                        };
                        notificationsDto.Add(notifyBySMS);
                    }
                }

                if (emails != null)
                {
                    int emailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
                    foreach (var email in emails)
                    {
                        NotificationLogPostDto notifyByMail = new NotificationLogPostDto()
                        {
                            NotificationChannelId = emailChannel,
                            UserId = userId,
                            Lang = lang.Trim().ToLower(),
                            NotificationTitle = title,
                            NotificationBody = body + code,
                            ToAddress = email,
                            ApplicationId = appId
                        };
                        notificationsDto.Add(notifyByMail);
                    }
                }

                await DoSend(notificationsDto, true, true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in generate OTP " + ex.Message);
                return false;
            }
        }

        public async Task<LogInResultDto> VerifyOTP(int userId, string number, string lang)
        {
            int otpPeriodInMinutes = Constants.OTP_PERIOD_If_MISSED_IN_APP_SETTING;

            if (_configuration["OtpPeriodInMinutes"] == null)
            {
                _logger.LogInformation("Warning!!! OtpPeriodInMinutes is missing");
            }
            else
            {
                bool success = int.TryParse(_configuration["OtpPeriodInMinutes"], out int settingPeriod);
                if (!success || settingPeriod < 1)
                {
                    _logger.LogInformation("Warning OtpPeriodInMinutes is invalid number or < 1 minute");
                }
                else
                {
                    otpPeriodInMinutes = settingPeriod;
                }
            }


            var lastOtp = await _EngineCoreDBContext.OtpLog.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (lastOtp == null || lastOtp.GeneratedDate.AddMinutes(otpPeriodInMinutes) < DateTime.Now)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ExpiredOTP"));
                throw _exception;
            }

            if (lastOtp.OtpCode.Trim() != number.Trim())
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "IncorrectOTP"));
                throw _exception;
            }

            LogInResultDto logInResultDto = new LogInResultDto();

            var user = _EngineCoreDBContext.User.Where(x => x.Id == userId).FirstOrDefault();

            if (user == null)
            {
                return logInResultDto;
            }

            LogInDtoLocal userDto = new LogInDtoLocal()
            {
                Email = user.Email,
                PassWord = "VerfiyByOTP"
            };

            logInResultDto = await _iUserRepository.VisitorSignIn(userDto, lang);
            return logInResultDto;
        }

        public async Task<string> GenerateUrlToken(int userId, int serviceId, int applicationId, string lang)
        {
            var user = await _EngineCoreDBContext.User.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            Claim[] claims = GenerateClames(user, serviceId, applicationId);
            string jwt = GenerateToken(claims, _jwt.Key);

            Guid guid = Guid.NewGuid();
            ShortenUrl shortenUrl = new ShortenUrl
            {
                GuidUrl = guid,
                Url = jwt
            };

            await _EngineCoreDBContext.ShortenUrl.AddAsync(shortenUrl);
            if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
            {
                return guid.ToString();
            }

            _exception.AttributeMessages.Add(Constants.getMessage(lang, "FailToken"));
            throw _exception;
        }

        public async Task<UserAppDto> VerifyToken(Guid guid, string lang)
        {
            IdentityModelEventSource.ShowPII = true;
            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,

                ValidAudience = _configuration["Jwt:Audience"].ToLower(),
                ValidIssuer = _configuration["Jwt:Issuer"].ToLower(),
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key))
            };
            var jwt = await _EngineCoreDBContext.ShortenUrl.Where(x => x.GuidUrl == guid).Select(x => x.Url).FirstOrDefaultAsync();
            if (jwt == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "MissedGUID"));
                throw _exception;
            }

            ClaimsPrincipal principal = new ClaimsPrincipal();
            try
            {
                principal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out validatedToken);
            }
            catch
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ExpiredToken"));
                throw _exception;
            }

            if (validatedToken.ValidTo < DateTime.Now)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ExpiredToken"));
                throw _exception;
            }

            if (validatedToken.ValidFrom > DateTime.Now)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "NotActivatedToken"));
                throw _exception;
            }

            if (principal.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "InvalidToken"));
                throw _exception;
            }

            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            string EmirateId = principal.Claims.Where(x => x.Type == "EmirateId").Select(x => x.Value).FirstOrDefault();
            string serviceId = principal.Claims.Where(x => x.Type == "serviceId").Select(x => x.Value).FirstOrDefault();
            string appId = principal.Claims.Where(x => x.Type == "appId").Select(x => x.Value).FirstOrDefault();

            if (userId == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }


            if (EmirateId == null || serviceId == null || appId == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "InvalidToken"));
                throw _exception;
            }

            ApplicationRepositiory isOredrExistAndOwner = new ApplicationRepositiory(_iApplicationPartyRepository, _loggerForApplication, _iTransactionRepository);
            if (!await isOredrExistAndOwner.IsParty(Int32.Parse(appId), userId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "unauthoraizedForAPP"));
                throw _exception;
            }

            var userApp = await _EngineCoreDBContext.Application.Where(x => x.Id == int.Parse(appId))
                                                             .Include(x => x.AppTransaction)
                                                             .ThenInclude(x => x.ApplicationParty).FirstOrDefaultAsync();

            if (userApp == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "unauthoraizedForAPP"));
                throw _exception;
            }

            var userParty = userApp.AppTransaction.ApplicationParty.ToList().Where(x => x.PartyId == userId).FirstOrDefault();

            if (userParty == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "unauthoraizedForAPP"));
                throw _exception;
            }

            var emails = new List<string>();
            var phones = new List<string>();
            var masterUser = await _EngineCoreDBContext.User.Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (masterUser.Email != null)
            {
                emails.Add(masterUser.Email);
            }

            if (userParty.AlternativeEmail != null)
            {
                if (!emails.Contains(userParty.AlternativeEmail))
                {
                    emails.Add(userParty.AlternativeEmail);
                }          
            }

            if (userParty.Email != null)
            {
                if (!emails.Contains(userParty.Email))
                {
                    emails.Add(userParty.Email);
                }
            }

            if (masterUser.PhoneNumber != null)
            {
                phones.Add(masterUser.PhoneNumber);
            }

            if (userParty.Mobile != null)
            {
                if (!phones.Contains(userParty.Mobile))
                {
                    phones.Add(userParty.Mobile);
                }
            }


            if (emails == null && phones == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "userHasNotAddresses"));
                throw _exception;
            }

            // the token is valid, Send OTP.
            if (!await SendOTP(userId, phones, emails, Int32.Parse(appId), lang))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "FiledInsendingOTP"));
                throw _exception;
            }

            UserAppDto res = new UserAppDto
            {
                ApplicationId = appId,
                ServiceId = serviceId,
                UserId = userId
            };

            return res;
        }

        private string GenerateToken(Claim[] claims, string key)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            var jwt = new JwtSecurityToken(
                _configuration["jwt:Issuer"].ToLower(),
                _configuration["jwt:Audience"].ToLower(),
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddDays(365));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private Claim[] GenerateClames(User user, int serviceId, int appId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("EmirateId", user.EmiratesId),
                new Claim("serviceId", serviceId.ToString()),
                new Claim("appId", appId.ToString()),
                new Claim(ClaimTypes.Expiration, new DateTimeOffset().ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            return claims.ToArray();
        }



        private string GetNotiBody(string ticketId, string appId, string time, string url)
        {
            return 
                " بخصوص الطلب رقم " + appId +
                " يرجى العلم ان رقم الدور هو  " + ticketId + 
                "   وان الوقت المتوقع للمقابلة سيبدا بعد  " + time +
                " دقيقة تقريبا يرجى تشغيل رابط الفيديو قبل الموعد المحدد وانتظار كاتب العدل " + url;

        }

        private async Task<List<UserInfo>> GetAppUsers(string applicationNo, int mailChannel, int smsChannel)
        {
            List<UserInfo> res = new List<UserInfo>();

            var userApp = await _EngineCoreDBContext.Application.Where(x => x.Id == Convert.ToInt32(applicationNo))
                                                 .Include(x => x.AppTransaction)
                                                 .ThenInclude(x => x.ApplicationParty).FirstOrDefaultAsync();
            if (userApp == null)
            {
                return res;
            }

            var applicationParties = userApp.AppTransaction.ApplicationParty.ToList();


            foreach (var applicationParty in applicationParties)
            {
                UserInfo userInfo = new UserInfo()
                {
                    UserId = (int)applicationParty.PartyId,
                    UserName = applicationParty.FullName
                };


                List<UserInfoDetails> userInfoDetails = new List<UserInfoDetails>();


                if (applicationParty.Email != null)
                {
                    UserInfoDetails userInfoDetail = new UserInfoDetails()
                    {
                        Address = applicationParty.Email,
                        ChannelNo = mailChannel
                    };
                    userInfoDetails.Add(userInfoDetail);
                }

                if (applicationParty.Mobile != null)
                {
                    UserInfoDetails userInfoDetail = new UserInfoDetails()
                    {
                        Address = applicationParty.Mobile,
                        ChannelNo = smsChannel
                    };
                    userInfoDetails.Add(userInfoDetail);
                }

                var userMain = await _EngineCoreDBContext.User.Where(x => x.Id == (int)applicationParty.PartyId).FirstOrDefaultAsync();
                if (userMain != null)
                {
                    if (userMain.PhoneNumber != null && userMain.PhoneNumber != applicationParty.Mobile)
                    {
                        UserInfoDetails userInfoDetail = new UserInfoDetails()
                        {
                            Address = userMain.PhoneNumber,
                            ChannelNo = smsChannel
                        };
                        userInfoDetails.Add(userInfoDetail);
                    }

                    if (userMain.Email != null && userMain.Email != applicationParty.Email)
                    {
                        UserInfoDetails userInfoDetail = new UserInfoDetails()
                        {
                            Address = userMain.Email,
                            ChannelNo = mailChannel
                        };
                        userInfoDetails.Add(userInfoDetail);
                    }
                }

                userInfo.Adresses = userInfoDetails;

                res.Add(userInfo);
            }

            return res;
        }

        public async Task NotifyQueue()
        {
            // get if inside working time.
            if (!await _iWorkingTimeRepository.IsInsideWorkingHours(DateTime.Now))
            {
                return;
            }

            var query = (
                         from ul in _EngineCoreDBContext.UserLogger.Where(x => x.LoggingDate.Date == DateTime.Now.Date)
                         group ul by ul.UserId into t
                         select new
                         {
                             loggingdate = t.Max(p => p.LoggingDate),
                             userid = (int)t.Key
                         });

            int OnlineEmployee = 0;
            foreach (var x in query)
            {
                if (await _EngineCoreDBContext.UserLogger.AnyAsync(y => y.LoggingDate == x.loggingdate && y.UserId == x.userid && y.StartWorkForEmployee == true))
                {
                    OnlineEmployee++;
                }
            }

            if (OnlineEmployee == 0)
            {
                return;
            }

            string MeetingBaseUrl = _configuration["MeetingBaseUrl"];

            int period = 20;
            var per = await _EngineCoreDBContext.ServiceKind.FirstOrDefaultAsync();
            if (per != null && per.EstimatedTimePerProcess != 0)
            {
                period = per.EstimatedTimePerProcess;
            }

            var allPendingProcesses = await (from queue in _EngineCoreDBContext.QueueProcesses.Where
                                             (
                                               x => x.Status == (int)Constants.PROCESS_STATUS.PENDING
                                            && x.ExpectedDateTime <= DateTime.Now.AddMinutes(3 * period))

                                             join application in _EngineCoreDBContext.Application
                                             on queue.ProcessNo equals application.Id.ToString()
                                             orderby queue.ExpectedDateTime.Date, queue.ProcessNo
                                             select new
                                             {
                                                 queue.TicketId,
                                                 queue.ProcessNo,
                                                 application.ServiceId,
                                                 queue.NotifyHighLevel,
                                                 queue.NotifyMediumLevel,
                                                 queue.NotifyLowLevel
                                             }).Take(OnlineEmployee * 3).ToListAsync();


            List<NotificationLogPostDto> notificationLogPostDto = new List<NotificationLogPostDto>();

            int mailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            int smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            for (int i = 0; i < allPendingProcesses.Count(); i++)
            {
                if (i < OnlineEmployee && allPendingProcesses[i].NotifyHighLevel == true)
                {
                    continue;
                }
                else if (i > OnlineEmployee && i < OnlineEmployee * 2 && allPendingProcesses[i].NotifyMediumLevel == true)
                {
                    continue;
                }
                else if (allPendingProcesses[i].NotifyLowLevel == true)
                {
                    continue;
                }

                List<UserInfo> appUsers = await GetAppUsers(allPendingProcesses[i].ProcessNo, mailChannel, smsChannel);
                if (appUsers.Count == 0)
                {
                    continue;
                }

                int extraTime = period;
                if (i / OnlineEmployee > 0)
                {
                    extraTime += ((int)i / OnlineEmployee) * period;
                }

                foreach (var notifyUser in appUsers)
                {
                    var str = await GenerateUrlToken(notifyUser.UserId, (int)allPendingProcesses[i].ServiceId, Convert.ToInt32(allPendingProcesses[i].ProcessNo), "en");
                    var url = $"{MeetingBaseUrl}{str}";

                    foreach (var subDetain in notifyUser.Adresses)
                    {
                        NotificationLogPostDto newNoti = new NotificationLogPostDto()
                        {
                            CreatedDate = DateTime.Now,
                            Lang = "ar",
                            NotificationTitle = " تذكير بموعد كاتب العدل ",
                            NotificationBody = GetNotiBody(allPendingProcesses[i].TicketId.ToString(), allPendingProcesses[i].ProcessNo, extraTime.ToString(), url),
                            NotificationChannelId = subDetain.ChannelNo,
                            ToAddress = subDetain.Address,
                            UserId = notifyUser.UserId,
                            ApplicationId = Convert.ToInt32(allPendingProcesses[i].ProcessNo)
                        };
                        notificationLogPostDto.Add(newNoti);
                    }
                }

                var que = await _EngineCoreDBContext.QueueProcesses.Where(x => x.ProcessNo == allPendingProcesses[i].ProcessNo).SingleOrDefaultAsync();
                if (i < OnlineEmployee)
                {
                    que.NotifyHighLevel = true;
                }
                else if (i > OnlineEmployee && i < OnlineEmployee * 2)
                {
                    que.NotifyMediumLevel = true;
                }
                else
                {
                    que.NotifyLowLevel = true;
                }             
                _EngineCoreDBContext.QueueProcesses.Update(que);
                await _EngineCoreDBContext.SaveChangesAsync();
            }

            if (notificationLogPostDto.Count() > 0)
            {
                await DoSend(notificationLogPostDto, false, true);
            }
        }

    }
}
