using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.UnifiedGateDto;
using EngineCoreProject.DTOs.UnifiedGatePostDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IUnifiedGateSubServicesRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nancy.Json;
using Newtonsoft.Json.Linq;

using PaymentServicePro;
using Service_UAEPassCall;
using SMART_khadamatiSubServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.UnifiedGateSubServicesRepository
{
    public class UnifiedGateSubServices : IUnifiedGateSubServicesRepository
    {
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly SignInWithUGateSettings _SignInWithUGateSettings;
        private readonly IUserRepository _IUserRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        private readonly UserManager<User> _userManager;
        ValidatorException _exception;
        public UnifiedGateSubServices(ISysValueRepository ISysValueRepository, IUserRepository iUserRepository, IHttpContextAccessor httpContext, IGeneralRepository iGeneralRepository, IOptions<SignInWithUGateSettings> signInWithUGateSettings, EngineCoreDBContext EngineCoreDBContext, UserManager<User> userManager)
        {
            _iGeneralRepository = iGeneralRepository;
            _httpContext = httpContext;
            _EngineCoreDBContext = EngineCoreDBContext;
            _SignInWithUGateSettings = signInWithUGateSettings.Value;
            _IUserRepository = iUserRepository;
            _ISysValueRepository = ISysValueRepository;
            _userManager = userManager;
            _exception = new ValidatorException();
        }

        public async Task<string> AccessAccountOnUnifiedGate(string lang, string EL_Eid)
        {
            string EmailID = await _EngineCoreDBContext.User.Where(x => x.EmiratesId == EL_Eid).Select(x => x.Email).FirstOrDefaultAsync();
            string URL = null;
            Dictionary<string, string> DictionaryURL = new Dictionary<string, string>();

            DictionaryURL.Add("EL_Portal", _SignInWithUGateSettings.EL_Portal);
            DictionaryURL.Add("EL_Eid", EL_Eid);
            DictionaryURL.Add("EL_EmailID", EmailID);
            DictionaryURL.Add("EL_Lang", lang);// Secret Key+ PortalId+ServiceID



            string SecreKey = _SignInWithUGateSettings.InCommingSecretKey;//please check if valid
            string StringToHashCide = SecreKey + DictionaryURL["EL_Portal"] + DictionaryURL["EL_Eid"] + DictionaryURL["EL_EmailID"];
            string hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCide, SecreKey);

            DictionaryURL.Add("UG_Hash", hashCode);
            URL = _iGeneralRepository.GenerateURL(DictionaryURL, _SignInWithUGateSettings.domain);
            return URL;
        }
        public string RemoteSignOut()
        {
            return _SignInWithUGateSettings.signOut;
        }

        public string GenerateURLForSignInWithUGate(string lang, string EL_Service, string theme)
        {
            string URL = null;
            Dictionary<string, string> DictionaryURL = new Dictionary<string, string>();
            DictionaryURL.Add("EL_Portal", _SignInWithUGateSettings.EL_Portal);
            DictionaryURL.Add("EL_Service", EL_Service);
            DictionaryURL.Add("EL_Lang", lang);// Secret Key+ PortalId+ServiceID
            string SecreKey = _SignInWithUGateSettings.OutCommingSecretKey;
            string StringToHashCide = SecreKey + DictionaryURL["EL_Portal"] + DictionaryURL["EL_Service"];
            string hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCide, SecreKey);

            DictionaryURL.Add("UG_Hash", hashCode);
            DictionaryURL.Add("theme", theme);
            URL = _iGeneralRepository.GenerateURL(DictionaryURL, _SignInWithUGateSettings.domain);
            return URL;
        }

        public async Task<PaymentGetStatusResponse> GetPaymentStatusAsync(string pruchaseId, string secureHash, int entityId)
        {
            PaymentServiceClient client = new PaymentServiceClient(PaymentServiceClient.EndpointConfiguration.SOAPEndPoint);
            PaymentGetStatusResponse response = await client.GetPaymentStatusAsync(pruchaseId, secureHash, entityId);
            return response;
        }

        public async Task<AdmService> GetServiceByUID(int ugId)
        {
            return await _EngineCoreDBContext.AdmService.Where(x => x.UgId == ugId).FirstOrDefaultAsync();
        }

        public UnifiedGateSubServicesDto GetSubServicesFromUnifiedGate()
        {
            khadamati_ServicesSoapClient client = new khadamati_ServicesSoapClient(khadamati_ServicesSoapClient.EndpointConfiguration.khadamati_ServicesSoap12);

            SMART_khadamatiSubServicesResponse response = client.SMART_khadamatiSubServicesAsync(789, 0).Result;
            string responseBody = response.Body.SMART_khadamatiSubServicesResult.ToString();
            //JObject json = JObject.Parse(response.Body.SMART_khadamatiSubServicesResult.ToString());
            JavaScriptSerializer j = new JavaScriptSerializer();
            UnifiedGateSubServicesDto SubServicesDescription = j.Deserialize<UnifiedGateSubServicesDto>(response.Body.SMART_khadamatiSubServicesResult.ToString());

            return SubServicesDescription;
        }



        public UnifiedGateUserInformationDto GetUserInformationUnifiedGate(UnifiedGateUserInformationPostDto userInfo)
        {
            Service_UAEPassCallSoapClient client =
                 new Service_UAEPassCallSoapClient(Service_UAEPassCallSoapClient.EndpointConfiguration.Service_UAEPassCallSoap12);
            String D = @"{'dtData':[{'Lang':'" + userInfo.dtData[0].Lang + @"','EmiratesId':'" + userInfo.dtData[0].EmiratesId + @"','EmailID':'" + userInfo.dtData[0].EmailID + @"' }]}";
            SMART_GetDetailsByEIDResponse response = client.SMART_GetDetailsByEIDAsync(D).Result;
            string responseBody = response.Body.SMART_GetDetailsByEIDResult.ToString();
            JObject json = JObject.Parse(response.Body.SMART_GetDetailsByEIDResult.ToString());
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            UnifiedGateUserInformationDto SubServicesDescription = javaScriptSerializer.Deserialize<UnifiedGateUserInformationDto>(response.Body.SMART_GetDetailsByEIDResult.ToString());

            if (SubServicesDescription.dtData[0].EmiratesID == null)
            {
                return null;
            }
            return SubServicesDescription;
        }


        public async Task<UserResultDto> UpdateOrInsertUserInfoFromUnifiedGate(UnifiedGateUserInformationPostDto userInfo, string lang)
        {
            UserResultDto userRes = new UserResultDto();
            var myUser = await _EngineCoreDBContext.User.Where(x => x.EmiratesId == userInfo.dtData[0].EmiratesId).FirstOrDefaultAsync();

            UnifiedGateUserInformationDto unifiedGateUserInformationDto = new UnifiedGateUserInformationDto();
            unifiedGateUserInformationDto = GetUserInformationUnifiedGate(userInfo);

            UserPostDto UserPostDto = new UserPostDto();
            if (unifiedGateUserInformationDto != null)
            {
                UserPostDto.UserName = unifiedGateUserInformationDto.dtData[0].UserName;
                UserPostDto.FullName = unifiedGateUserInformationDto.dtData[0].FullName;
                UserPostDto.Email = unifiedGateUserInformationDto.dtData[0].Email;

                // user password is encrypted by UG, so create a random password.
                //UserPostDto.PasswordHash = unifiedGateUserInformationDto.dtData[0].UserPassword;
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();
                var rnd = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
                UserPostDto.PasswordHash = "P@ssw0rd_" + rnd;

                UserPostDto.SecurityQuestionId = unifiedGateUserInformationDto.dtData[0].SecurityQuestionID;
                UserPostDto.SecurityQuestionAnswer = unifiedGateUserInformationDto.dtData[0].SecurityQuestionAnswer;
                try
                {
                    UserPostDto.BirthdayDate = DateTime.ParseExact(unifiedGateUserInformationDto.dtData[0].DOB, "dd/MM/yyyy", null);
                }
                catch
                {
                    UserPostDto.BirthdayDate = null; 
                }
                UserPostDto.NatId = unifiedGateUserInformationDto.dtData[0].NatID;
                UserPostDto.TelNo = unifiedGateUserInformationDto.dtData[0].TelNo;
                UserPostDto.Status = unifiedGateUserInformationDto.dtData[0].Status;
                UserPostDto.EmailLang = unifiedGateUserInformationDto.dtData[0].Email_Lang;
                UserPostDto.SmsLang = unifiedGateUserInformationDto.dtData[0].SMS_Lang;
                UserPostDto.AreaId = unifiedGateUserInformationDto.dtData[0].AreaID;
                UserPostDto.Gender = unifiedGateUserInformationDto.dtData[0].Gender;
                UserPostDto.NotificationType = unifiedGateUserInformationDto.dtData[0].NotificationType;
                UserPostDto.ProfileStatus = unifiedGateUserInformationDto.dtData[0].ProfileStatus;
                UserPostDto.Address = unifiedGateUserInformationDto.dtData[0].Address;
                UserPostDto.EmiratesId = unifiedGateUserInformationDto.dtData[0].EmiratesID;
                UserPostDto.PhoneNumber = unifiedGateUserInformationDto.dtData[0].MobileNo;

                if (myUser != null)
                {
                    var oldRoles = await _userManager.GetRolesAsync(myUser);
                    if (oldRoles.Contains(Constants.DefaultUserPolicy))
                    {
                        userRes = await _IUserRepository.EditUser(myUser.Id, UserPostDto, true, false, lang);
                    }
                    else
                    {
                        var publicRole = await _EngineCoreDBContext.Roles.Where(x => x.Name == Constants.DefaultUserPolicy).Select(x => x.Id).FirstOrDefaultAsync();
                        UserPostDto.UserRoles = new List<int>
                        {
                        publicRole
                        };
                        userRes = await _IUserRepository.EditUser(myUser.Id, UserPostDto, true, true, lang);
                    }
                }
                else
                {
                    // Don't check if email is existed before is not allowed, canceled by Bshar.
                    // so create a new user and convert the exist one to invalid if exist.
                    var publicRole = await _EngineCoreDBContext.Roles.Where(x => x.Name == Constants.DefaultUserPolicy).Select(x => x.Id).FirstOrDefaultAsync();
                    UserPostDto.UserRoles = new List<int>
                        {
                        publicRole
                        };
                    userRes = await _IUserRepository.CreateUser(UserPostDto, null, true, lang, true);
                }
            }
            return userRes;
        }

        public async Task<LogInResultDto> remotelogin(RemoteLoginDto remoteLoginDto, string lang)
        {
            LogInResultDto logInResultDto;
            string SecreKey = _SignInWithUGateSettings.InCommingSecretKey.Trim();
            string StringToHashCode = null;// SecreKey.Trim() + remoteLoginDto.MobileNumber.Trim() + remoteLoginDto.EmiratesId.Trim();// +"0000"+ remoteLoginDto.Email.Trim();                                    
            string hashCode = null;
            UnifiedGateUserInformationPostDto userInfo = new UnifiedGateUserInformationPostDto();
            userInfo.dtData[0].EmailID = remoteLoginDto.Email;
            userInfo.dtData[0].EmiratesId = remoteLoginDto.EmiratesId;
            if (remoteLoginDto.Lang == "en")
            {
                userInfo.dtData[0].Lang = 1.ToString();
            }
            else
            {
                userInfo.dtData[0].Lang = 0.ToString();
            }
            var UpdatedUser = await UpdateOrInsertUserInfoFromUnifiedGate(userInfo, lang);
            if (UpdatedUser.user != null)
            {
                LogInDtoUg LogInDtoUg = new LogInDtoUg() { Email = UpdatedUser.user.Email, PassWord = UpdatedUser.user.PasswordHash };
                if (remoteLoginDto.ServiceId == "0")
                {
                    LogInDtoUg.ServiceId = 0;
                    LogInDtoUg.AppId = null;
                    StringToHashCode = SecreKey.Trim() + remoteLoginDto.MobileNumber.Trim() + remoteLoginDto.EmiratesId.Trim();// +remoteLoginDto.ServiceId.Trim()+ remoteLoginDto.Email.Trim();  
                    hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCode, SecreKey);
                    if (hashCode != remoteLoginDto.Hash)
                    {
                        _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserUGPermissing"));
                        throw _exception;
                    }
                }
                else
                {
                    List<AdmService> resultFromServices = null;
                    List<Application> resultFromApplication = null;

                    try
                    {
                        resultFromApplication = await _EngineCoreDBContext.Application.Where(x => x.ApplicationNo == remoteLoginDto.ServiceId.ToString()).ToListAsync();
                    }
                    catch (Exception e)
                    {
                        resultFromApplication = null;
                    }

                    try
                    {
                        resultFromServices = await _EngineCoreDBContext.AdmService.Where(x => x.UgId == Convert.ToInt32(remoteLoginDto.ServiceId)).ToListAsync();
                    }
                    catch (Exception e)
                    {
                        resultFromServices = null;
                    }

                    if (resultFromApplication != null)
                    {
                        if (resultFromApplication.Count > 0)
                        {
                            LogInDtoUg.AppId = await GetApplicationIdByAppNoAsync(remoteLoginDto.ServiceId);// in this case service id == application no
                            LogInDtoUg.ServiceId = await GetserviceIdByAppNoAsync(remoteLoginDto.ServiceId);//get service id by application no

                            StringToHashCode = SecreKey.Trim() + remoteLoginDto.MobileNumber.Trim() + remoteLoginDto.EmiratesId.Trim() + remoteLoginDto.ServiceId.Trim() + remoteLoginDto.Email.Trim();
                            hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCode, SecreKey);
                            if (hashCode != remoteLoginDto.Hash)
                                return new LogInResultDto
                                {
                                    StatusCode = "401",
                                };

                            logInResultDto = await _IUserRepository.UGSignIn(LogInDtoUg, lang);

                            return logInResultDto;
                        }
                    }
                    if (resultFromServices != null)
                    {
                        if (resultFromServices.Count > 0)
                        {
                            LogInDtoUg.ServiceId = await GetLocalServiceIdAsync(remoteLoginDto.ServiceId);//get service id by UG service id;
                            LogInDtoUg.AppId = null;

                            StringToHashCode = SecreKey.Trim() + remoteLoginDto.MobileNumber.Trim() + remoteLoginDto.EmiratesId.Trim();// + remoteLoginDto.ServiceId.Trim() + remoteLoginDto.Email.Trim();
                            hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCode, SecreKey);
                            if (hashCode != remoteLoginDto.Hash)
                                return new LogInResultDto
                                {
                                    StatusCode = "401",
                                };


                            logInResultDto = await _IUserRepository.UGSignIn(LogInDtoUg, lang);
                            return logInResultDto;
                        }
                    }
                }

                LogInDtoUg.ServiceId = 0;
                LogInDtoUg.AppId = null;
                hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCode, SecreKey);
                if (hashCode != remoteLoginDto.Hash)
                    return new LogInResultDto
                    {
                        StatusCode = "401",
                    };
                logInResultDto = await _IUserRepository.UGSignIn(LogInDtoUg, lang);
                return logInResultDto;
            }


            return new LogInResultDto
            {
                StatusCode = "404",
            };

        }

        private async Task<int?> GetserviceIdByAppNoAsync(string serviceId)
        {
            int? ServiceId = null;
            try
            {
                ServiceId = await _EngineCoreDBContext.Application.Where(x => x.ApplicationNo == serviceId).Select(x => x.ServiceId).FirstOrDefaultAsync();

            }
            catch (Exception e)
            {

                ServiceId = null;
            }
            return ServiceId;
        }

        private async Task<int?> GetApplicationIdByAppNoAsync(string serviceId)
        {
            int? appId = null;
            try
            {
                appId = await _EngineCoreDBContext.Application.Where(x => x.ApplicationNo == serviceId).Select(x => x.Id).FirstOrDefaultAsync();

            }
            catch (Exception e)
            {

                appId = null;
            }
            return appId;
        }

        private async Task<int> GetLocalServiceIdAsync(string serviceId)
        {
            int? id = 0;
            try
            {
                id = await _EngineCoreDBContext.AdmService.Where(x => x.UgId == Convert.ToInt32(serviceId)).Select(x => x.Id).FirstOrDefaultAsync();
                if (id == null)
                    id = 0;
            }
            catch (Exception e)
            {
                id = 0;
            }

            return (int)id;
        }

        public string RedirectToUGDahBoard(string Lang, string theme)
        {


            string Email = _IUserRepository.GetUserEmail();
            string Eid = _IUserRepository.GetUserEid();


            string URL = null;
            Dictionary<string, string> DictionaryURL = new Dictionary<string, string>();
            DictionaryURL.Add("EL_Portal", _SignInWithUGateSettings.EL_Portal);
            DictionaryURL.Add("EL_Eid", Eid);
            DictionaryURL.Add("EL_EmailID", Email);
            DictionaryURL.Add("EL_Lang", Lang);
            string SecreKey = _SignInWithUGateSettings.OutCommingSecretKey;

            //  string StringToHashCide = SecreKey + "8" + "784198402171619" + "j.cherian@smartv.ae";// for test 
            string StringToHashCide = SecreKey + DictionaryURL["EL_Portal"] + DictionaryURL["EL_Eid"] + DictionaryURL["EL_EmailID"];

            string hashCode = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHashCide, SecreKey);

            DictionaryURL.Add("UG_Hash", hashCode);
            DictionaryURL.Add("theme", theme);
            URL = _iGeneralRepository.GenerateURL(DictionaryURL, _SignInWithUGateSettings.domain);
            return URL;
        }

        public async Task<responseCardDashBoardDto> statisticsDashboard(DashBoardRequestDto dashBoardRequestDto)
        {
            string EmiratesId = dashBoardRequestDto.dtData[0].EmiratesId;
            string Email = dashBoardRequestDto.dtData[0].EmailID;
            int? UserId = await _EngineCoreDBContext.User.Where(x => x.EmiratesId == EmiratesId && x.Email == Email).Select(x => x.Id).FirstOrDefaultAsync();

            if (UserId == null || UserId == 0)
            {
                return new responseCardDashBoardDto { status = 0, responseCardDashBoard = null };
            }

            DateTime FromDate = Convert.ToDateTime("1970/01/01");
            DateTime ToDate = Convert.ToDateTime("2200/01/01"); ;

            /*  Disabled accord to new requirements from UG.
            try
            {
                FromDate = Convert.ToDateTime(dashBoardRequestDto.dtData[0].FromDate);
            }
            catch (Exception e)
            {
                FromDate = Convert.ToDateTime("1970/01/01");
            }
            try
            {
                ToDate = Convert.ToDateTime(dashBoardRequestDto.dtData[0].ToDate);
            }
            catch (Exception e)
            {
                ToDate = Convert.ToDateTime("2200/01/01");
            }
            */

            var result = from applicationParty in _EngineCoreDBContext.ApplicationParty
                         where
             applicationParty.IsOwner == true && applicationParty.PartyId == UserId
                         select new
                         {
                             applicationParty.PartyId,
                             applicationParty.IsOwner,
                             applicationParty.TransactionId,
                             transaction_id2 = applicationParty.Transaction.Id,
                             application_id = (int?)applicationParty.Transaction.ApplicationId,
                             application_id2 = applicationParty.Transaction.Application.Id,
                             created_date = (DateTime?)applicationParty.Transaction.Application.CreatedDate,
                             state_id = (int?)applicationParty.Transaction.Application.StateId
                         };

            var finalResult = result.Where(x => x.created_date >= FromDate && x.created_date <= ToDate);


            int ONPROGRESSId = await _ISysValueRepository.GetIdByShortcut("ONPROGRESS");
            int REJECTED = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int DONE = await _ISysValueRepository.GetIdByShortcut("DONE");
            int RETURNED = await _ISysValueRepository.GetIdByShortcut("RETURNED");


            int ONPROGRESSTotal = finalResult.Where(x => x.state_id == ONPROGRESSId || x.state_id == RETURNED).Count();
            int REJECTEDCount = finalResult.Where(x => x.state_id == REJECTED).Count();
            int DONECount = finalResult.Where(x => x.state_id == DONE).Count();

            int Total = finalResult.Where(x => x.state_id == ONPROGRESSId || x.state_id == RETURNED || x.state_id == DONE || x.state_id == REJECTED).Count();

            responseCardDashBoard responseCardDashBoard = new responseCardDashBoard();
            responseCardDashBoard.dtOut = new Dtout[] { new Dtout() };

            responseCardDashBoard.dtOut[0].Approved = DONECount.ToString();
            responseCardDashBoard.dtOut[0].Rejected = REJECTEDCount;

            responseCardDashBoard.dtOut[0].Total = Total;

            responseCardDashBoard.dtOut[0].UnderProcessing = ONPROGRESSTotal.ToString();

            return new responseCardDashBoardDto { status = 1, responseCardDashBoard = responseCardDashBoard };


        }
        public async Task<responseTableDashBoardDto> statisticsTable(DashBoardRequestDto dashBoardRequestDto)
        {
            string Lang = dashBoardRequestDto.dtData[0].Lang;
            string ApplicationNo = dashBoardRequestDto.dtData[0].ApplicationNo;

            if (ApplicationNo == "-1") ApplicationNo = null;

            string EmiratesId = dashBoardRequestDto.dtData[0].EmiratesId;

            string Email = dashBoardRequestDto.dtData[0].EmailID;
            List<int?> Status = await GetLocalStatusIdAsync(dashBoardRequestDto.dtData[0].Status);

            User user = await _EngineCoreDBContext.User.Where(x => x.EmiratesId == EmiratesId && x.Email == Email).FirstOrDefaultAsync();
            int UserId = user.Id;

            if (user == null) return new responseTableDashBoardDto { status = 0, responseTableDashBoard = null };

            DateTime FromDate = Convert.ToDateTime("1970/01/01");
            DateTime ToDate = Convert.ToDateTime("2200/01/01"); ;

            /*  Disabled accord to new requirements from UG.
            try
            {
                FromDate = Convert.ToDateTime(dashBoardRequestDto.dtData[0].FromDate);
            }
            catch (Exception e)
            {
                FromDate = Convert.ToDateTime("1970/01/01");
            }
            try
            {
                ToDate = Convert.ToDateTime(dashBoardRequestDto.dtData[0].ToDate);
            }
            catch (Exception e)
            {
                ToDate = Convert.ToDateTime("2200/01/01");
            }
            */

            var result = (from application_party in _EngineCoreDBContext.ApplicationParty
                          where application_party.PartyId == UserId && application_party.IsOwner == true &&
                            application_party.Transaction.Application.ApplicationNo == (ApplicationNo == null ? application_party.Transaction.Application.ApplicationNo : ApplicationNo) &&
                            application_party.Transaction.Application.CreatedDate >= (FromDate == null ? application_party.Transaction.Application.CreatedDate : FromDate) &&
                            application_party.Transaction.Application.CreatedDate <= (ToDate == null ? application_party.Transaction.Application.CreatedDate : ToDate)

                          select new
                          {
                              application_party.PartyId,
                              application_party.IsOwner,
                              application_party.Email,
                              application_party.EmiratesIdNo,
                              application_id = (int?)application_party.Transaction.ApplicationId,
                              service_id = (int?)application_party.Transaction.Application.ServiceId,
                              application_party.Transaction.Application.ApplicationNo,
                              state_id = (int?)application_party.Transaction.Application.StateId,
                              created_date = (DateTime?)application_party.Transaction.Application.CreatedDate
                          }).ToList().Where(x => Status.Contains(x.state_id)).Select(x =>
                             new Maindata
                             {
                                 AppSource = Constants.UnifiedGateEnotarySystemCode,
                                 ApplicationNo = x.ApplicationNo,

                                 ApplicationDate =Convert.ToDateTime( x.created_date).ToString("yyyy/MM/dd HH:MM"),
                                 Status = GetUGStatusCodeByLocalStatusId(x.state_id, Lang).Result,
                                 StatusCode = GetUGStatusIdByLocalStatusIdAsync(x.state_id).Result,
                                 Center = Constants.UnifiedGateCenterName,
                                 Service = GethServiceName(x.service_id, Lang).Result,
                                 Amount = "0",
                                 PortalId = Constants.UnifiedGateEnotarySystemId,
                                 UniqueId = x.application_id.ToString(),
                                 UserEmail = user.Email,
                                 UserNationalID = user.NatId.ToString(),
                                 UserMobileNo = user.PhoneNumber,
                             }).OrderByDescending(u=>u.ApplicationDate).ToList();


            responseTableDashBoardDto responseTableDashBoard = new responseTableDashBoardDto { status = 1, responseTableDashBoard = new responseTableDashBoard { MainData = result } };
            return responseTableDashBoard;
        }

        private async Task<string> GethServiceName(int? serviceId, string lang)
        {

            string ServiceShortCut = await _EngineCoreDBContext.AdmService.Where(x => x.Id == serviceId).Select(x => x.Shortcut).FirstOrDefaultAsync();
            string ServiceEnTrans = null;
            switch (lang)
            {
                case "0":
                    return ServiceEnTrans = await _iGeneralRepository.GetTranslateByShortCut("en", ServiceShortCut);
                case "1":
                    return ServiceEnTrans = await _iGeneralRepository.GetTranslateByShortCut("ar", ServiceShortCut);
                default:
                    return ServiceEnTrans = await _iGeneralRepository.GetTranslateByShortCut("en", ServiceShortCut);
            }
        }

        private async Task<List<int?>> GetLocalStatusIdAsync(string status)
        {
            List<int?> StatusList = new List<int?>();

            int ONPROGRESSId = await _ISysValueRepository.GetIdByShortcut("ONPROGRESS");
            int RETURNED = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int REJECTED = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int DONE = await _ISysValueRepository.GetIdByShortcut("DONE");

            //if (status.Trim() == Constants.UnifiedGateUnderProcessing.Trim() || status.Trim() == Constants.UnifiedGateUnderProcessingAR.Trim())
            if (status.Trim() == Constants.UGStatus["Under Processing"].Trim()) //UPDATED
            { StatusList.Add(ONPROGRESSId); StatusList.Add(RETURNED); }
            //if (status.Trim() == Constants.UnifiedGateRejected.Trim() || status.Trim() == Constants.UnifiedGateRejectedAR.Trim())
            if (status.Trim() == Constants.UGStatus["Rejected"])
            { StatusList.Add(REJECTED); };

            //if (status.Trim() == Constants.UnifiedGateApproved.Trim() || status.Trim() == Constants.UnifiedGateApprovedAR.Trim())
            if (status.Trim() == Constants.UGStatus["Approved"])//
            {
                StatusList.Add(DONE);
            };//


            if (status.Trim() == Constants.UnifiedGateWithoutStatus.Trim())
            { StatusList.Add(REJECTED); StatusList.Add(ONPROGRESSId); StatusList.Add(RETURNED); StatusList.Add(DONE); };//UnifiedGateWithoutStatus

            return StatusList;
        }


        private async Task<string> GetUGStatusCodeByLocalStatusId(int? LocalStatusId, string lang)
        {
            string st = null;
            if (LocalStatusId == null) return st;
            string LocalShortCut = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == LocalStatusId).Select(x => x.Shortcut).FirstOrDefaultAsync(); ;
            if (LocalShortCut == "ONPROGRESS" || LocalShortCut == "RETURNED")
                if (lang == "1") st = Constants.UnifiedGateUnderProcessingAR; else st = Constants.UnifiedGateUnderProcessing;
            if (LocalShortCut == "DONE") { if (lang == "1") st = Constants.UnifiedGateApprovedAR; else st = Constants.UnifiedGateApproved; };
            if (LocalShortCut == "REJECTED") { if (lang == "1") st = Constants.UnifiedGateRejectedAR; else st = Constants.UnifiedGateRejected; };
            return st;
        }

        private async Task<string> GetUGStatusIdByLocalStatusIdAsync(int? LocalStatusId)
        {
            string st = null;

            string LocalShortCut = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == LocalStatusId).Select(x => x.Shortcut).FirstOrDefaultAsync(); ;


            if (LocalShortCut == "ONPROGRESS" || LocalShortCut == "RETURNED") st = Constants.UnifiedGateUnderProcessingID;
            if (LocalShortCut == "DONE") { st = Constants.UnifiedGateApprovedID; };
            if (LocalShortCut == "REJECTED") { st = Constants.UnifiedGateRejectedID; };
            return st;
        }

    }
}

