using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.SMSDto;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngineCoreProject.DTOs.ActionDtos;
using EngineCoreProject.DTOs.AdmService;

using Microsoft.Extensions.Options;
using System.Text;


using EngineCoreProject.Services.Job;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using SMSServiceReference;

using System.Text.RegularExpressions;

namespace EngineCoreProject.Services.NotificationService
{
    class SMSNotification : INotificationObserver
    {
        private readonly List<NotificationLogPostDto> _notificationsLogPostDto;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly ChannelSMSSetting _sMSSettings;
        private readonly IGeneralRepository _iGeneralRepository;

        public SMSNotification(List<NotificationLogPostDto> notificationsLogPostDto, EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, IOptions<ChannelSMSSetting> smsSetting)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _sMSSettings = smsSetting.Value;
            _notificationsLogPostDto = new List<NotificationLogPostDto>();
            _notificationsLogPostDto = notificationsLogPostDto;
            _iGeneralRepository = iGeneralRepository;
        }

        public async Task<List<NotificationLogPostDto>> Notify(bool sendImmediately)
        {
            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            foreach (var notify in _notificationsLogPostDto)
            {
                sendSMSListResponse sendSMS = new sendSMSListResponse();
                try
                {
                    //if (!Regex.Match(notify.ToAddress, @"^(\00971[0-9]{9})$").Success)
                    //{
                    //   throw new InvalidOperationException(String.Format("Invalid UAE phone number {0}", notify.ToAddress));
                    //}

                    if (sendImmediately)
                    {
                        SMSServiceSoapClient client = new SMSServiceSoapClient(SMSServiceSoapClient.EndpointConfiguration.SMSServiceSoap12);
                        AuthenticationHeader AuthenticationHeader = new AuthenticationHeader
                        {
                            UserName = _sMSSettings.UserName, // equal eLawyer
                            Password = _sMSSettings.Password  // equal eLawyerSMS
                        };

                        //  string utf8_String = notify.NotificationBody;
                        string priority = Constants.DEFAULT_PRIROITY_SMS;
                        string priorityFromSetting = "";
                        if (notify.NotificationBody.Contains(Constants.OTP_BODY_AR) || notify.NotificationBody.Contains(Constants.OTP_BODY_EN))
                        {
                            priorityFromSetting = _sMSSettings.HighProritySMS;
                        }
                        else
                        {
                            priorityFromSetting = _sMSSettings.LowProritySMS;
                        }

                        bool success = int.TryParse(priorityFromSetting, out int settingPriority);
                        if (success && settingPriority > 0 && settingPriority < 6)
                        {
                            priority = settingPriority.ToString();
                        }


                        var tempReportId = _iGeneralRepository.GetNewValueBySec().ToString();
                        //client.sendSMSListAsync method send SMS : All parameters are required and an error will occur if one of them is missed

                        sendSMS = await client.sendSMSListAsync(
                                                                 AuthenticationHeader,
                                                                 _sMSSettings.SenderID, // SenderID Equal eLawyer
                                                                 _sMSSettings.SourceSystemId, // SourceSystemId Equal 1
                                                                 tempReportId, // unique number on the notary system level and we get it through DB sequence 
                                                                 notify.NotificationBody, // message body
                                                                 notify.ToAddress, //  Emirates phone number WITH  ZIP code for example :0097150605****
                                                                 notify.Lang.Trim().ToUpper(), // Equal ( AR or EN ) capital letters (very important) and with trim white space (very important)
                                                                 priority,  // High Priority attribute from 1..4
                                                                 "N", //ScheduledMSG parameter Equal eLawyer
                                                                 "20210201011010", // FromDate parameter ( form (YYYYMMDDHHMMSS)) 
                                                                 "20410301011010", //  ToDate parameter ( form (YYYYMMDDHHMMSS))
                                                                 null);

                        string idMessage = sendSMS.sendSMSListResult.Records[0].MessageID.ToString();

                        notify.ReportValueId = idMessage;

                        // TODO check if sent or not
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                        notify.SentCount += 1;
                        notify.SendReportId += " Message is sent the id is " + tempReportId;
                    }
                    else
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                        notify.SentCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    string mes = "";
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                    notify.SentCount += 1;
                    if (sendSMS.sendSMSListResult != null)
                    {
                        mes = sendSMS.sendSMSListResult.responseDescription;
                    }

                    notify.SendReportId += String.Format(" Error at attempt {0}, : {1} ,{2}", notify.SentCount, ex.Message, mes);
                }

                res.Add(notify);
                Console.WriteLine("Notify through SMS");

            }
            return res;
        }


    }
}
