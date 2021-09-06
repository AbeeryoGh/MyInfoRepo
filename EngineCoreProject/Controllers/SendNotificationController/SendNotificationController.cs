using System;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using EngineCoreProject.DTOs;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EngineCoreProject.Services;
using SMSServiceReference;
using EngineCoreProject.DTOs.SMSDto;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using EngineCoreProject.IRepository.IUserRepository;

namespace EngineCoreProject.Controllers.Email
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendNotificationController : ControllerBase
    {
        private readonly ISendNotificationRepository _NotificationService;
        private readonly INotificationLogRepository _INotificationLogRepository;
        private readonly ChannelSMSSetting _sMSSettings;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IUserRepository _iUserRepository;
        public SendNotificationController(IGeneralRepository IGeneralRepository,IOptions<ChannelSMSSetting> smsSetting,ISendNotificationRepository sendNotification,
                                          IUserRepository iUserRepository, INotificationLogRepository iNotificationLogRepository)
        {
            _NotificationService = sendNotification;
            _INotificationLogRepository = iNotificationLogRepository;
            _sMSSettings = smsSetting.Value;
            _iUserRepository = iUserRepository;
            _iGeneralRepository = IGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("SendNotifications")]
        public async Task<IActionResult> SendNotifications(List<NotificationLogPostDto> notifications)
        {
            try
            {
                await _NotificationService.DoSend(notifications, true);
                return this.StatusCode(StatusCodes.Status200OK, notifications);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status200OK, "error occurred");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("ReSendNotifications")]
        public async Task<IActionResult> ReSendNotifications()
        {
            try
            {
                await _NotificationService.ReSend();
                return this.StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status200OK, "error occurred");
            }
        }

       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("NotifyQueue")]
        public async Task<IActionResult> NotifyQueue()
        {
            try
            {
                await _NotificationService.NotifyQueue();
                return this.StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status200OK, "error occurred");
            }
        }


        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("GenerateUrlToken")]
        public async Task<IActionResult> GenerateUrlToken(int userId, int serviceId, int appId, [FromHeader] string lang)
        {

            await _NotificationService.GenerateUrlToken(userId, serviceId, appId, lang);
            return this.StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost("VerifyToken")]
        public async Task<IActionResult> VerifyToken(Guid guid, [FromHeader] string lang)
        {

            var obj = await _NotificationService.VerifyToken(guid, lang);
            return this.StatusCode(StatusCodes.Status200OK, obj);
        }


        [HttpPost("VerifyOTPWithSignIn")]
        public async Task<IActionResult> VerifyOTPWithSignIn(int userId, string number, [FromHeader] string lang)
        {
            var obj = await _NotificationService.VerifyOTP(userId, number, lang);
            return this.StatusCode(StatusCodes.Status200OK, obj);
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddNotificationsLog")]
        public async Task<IActionResult> AddNotificationsLog(List<NotificationLogPostDto> notifications)
        {
            try
            {
                await _INotificationLogRepository.AddNotificationsLog(notifications);
                return this.StatusCode(StatusCodes.Status200OK, notifications);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status200OK, "error occurred");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetApplicationNotificationsLog")]
        public async Task<IActionResult> GetApplicationNotificationsLog(int applicationId, [FromHeader] string lang)
        {
            var result = await _INotificationLogRepository.GetApplicationNotificationsLog(applicationId, lang);
            return Ok(result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetInternalNotificationsLog")]
        public async Task<IActionResult> GetInternalNotificationsLog([FromHeader] string lang)
        {
            var res = await _INotificationLogRepository.GetInternalNotificationsLog(_iUserRepository.GetUserID(), lang);
            return Ok(res);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ReadInternalNotificationsLog")]
        public async Task<IActionResult> ReadInternalNotificationsLog(int notifyId)
        {
            var res = await _INotificationLogRepository.ReadInternalNotificationsLog(_iUserRepository.GetUserID(), notifyId);
            return Ok(res);
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageInfoReq MessageInfoReq)
        {
            sendSMSListResponse sendSMSListResponse1 = null;
            string utf8_String = null;
            try
            {

                    SMSServiceSoapClient client = new SMSServiceSoapClient(SMSServiceSoapClient.EndpointConfiguration.SMSServiceSoap12);
                    AuthenticationHeader AuthenticationHeader = new AuthenticationHeader
                    {
                        UserName = _sMSSettings.UserName, // equal eLawyer
                        Password = _sMSSettings.Password  // equal eLawyerSMS
                    };

                    utf8_String = MessageInfoReq.MessageToSend;
                    var tempReportId = _iGeneralRepository.GetNewValueBySec().ToString();
                    //client.sendSMSListAsync method send SMS : All parameters are required and an error will occur if one of them is missed

                      //sendSMSListResponse1 = await client.sendSMSListAsync(
                      //                                                                          AuthenticationHeader,
                      //                                                                          _sMSSettings.SenderID, // SenderID Equal eLawyer
                      //                                                                          _sMSSettings.SourceSystemId, // SourceSystemId Equal 1
                      //                                                                          tempReportId, // unique number on the notary system level and we get it through DB sequence 
                      //                                                                          utf8_String, // message body
                      //                                                                          MessageInfoReq.PhoneNumber, //  Emirates phone number WITH  ZIP code for example :0097150605****
                      //                                                                          MessageInfoReq.Lang.Trim().ToUpper(), // Equal ( AR or EN ) capital letters (very important) and with trim white space (very important)
                      //                                                                          "1",  // Priority attribute Equal eLawyer
                      //                                                                          "N", //ScheduledMSG parameter Equal eLawyer
                      //                                                                          "20210201011010", // FromDate parameter ( form (YYYYMMDDHHMMSS)) 
                      //                                                                          "20410301011010", //  ToDate parameter ( form (YYYYMMDDHHMMSS))
                      //                                                                          null);


                MessageInfoRes MessageInfoRes = new MessageInfoRes();
                MessageInfoRes.Lang = MessageInfoReq.Lang;
                MessageInfoRes.NumberOfCharacters = utf8_String.Length;

                MessageInfoRes.ResponseMessage= sendSMSListResponse1 ;


                return this.StatusCode(StatusCodes.Status200OK, MessageInfoRes);
            }
            catch (Exception)
            {
                MessageInfoRes MessageInfoRes = new MessageInfoRes();
                MessageInfoRes.Lang = MessageInfoReq.Lang;
                MessageInfoRes.NumberOfCharacters = utf8_String.Length;

                MessageInfoRes.ResponseMessage =  sendSMSListResponse1 ;
                return this.StatusCode(StatusCodes.Status500InternalServerError, MessageInfoRes);
            }
        }
    }
}

