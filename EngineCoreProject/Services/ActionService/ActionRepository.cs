/*
using EngineCoreProject.DTOs.ActionDtos;
using EngineCoreProject.DTOs.ChannelDto;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.IActionRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static EngineCoreProject.Services.ActionService.ActionRepository;

namespace EngineCoreProject.Services.ActionService
{
    public class ActionRepository : IActionRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public ActionRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;

        }

        public async Task<Dictionary<string, string>> SendNotification(AcuallNotification AcuallNotificationInfo)
        {
            Dictionary<string, string> DictionaryMessage = new Dictionary<string, string>();
            int? senderId = AcuallNotificationInfo.senderId;
            foreach (AcualNotificationReciverInfo acuallNotification in AcuallNotificationInfo.Notifications)
            {

                int? reciverId = acuallNotification.reciverId;
                User UserInfo = _EngineCoreDBContext.User.Where(x => x.Id == reciverId).FirstOrDefault();
                foreach (AcualNotificationMessageInfo acualNotificationMessageInfo in acuallNotification.NotificationMessageInfo)
                {
                    NotificationLog notification = new NotificationLog();

                    notification.NotificationTitle = acualNotificationMessageInfo.title;
                    notification.NotificationBody = acualNotificationMessageInfo.body;
                    notification.Lang = acualNotificationMessageInfo.lang;


                    // notification.NotificationChannelId = acualNotificationMessageInfo.channel_id;

                    _iGeneralRepository.Add(notification);

                    if (await _iGeneralRepository.Save())
                    {
                        DictionaryMessage.Add("ntification with id " + notification.Id, " was added successfully");


                        int? notificationId = notification.Id;
                        int? channelId = acualNotificationMessageInfo.channel_id;//notification_channel1
                        int? EmailShortCutIdInTable = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == "notification_channel1").Select(x => x.Id).FirstOrDefault();
                        int? SmsShortCutIdInTable = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == "notification_channel2").Select(x => x.Id).FirstOrDefault();
                        if (channelId == EmailShortCutIdInTable)
                        {
                            NotaryMessage notaryMessage = new NotaryMessage();
                            notaryMessage.Mobile = UserInfo.MobileNo;
                            notaryMessage.IsSent = 0;
                            notaryMessage.SendCount = 0;
                            notaryMessage.MessageValueId = notificationId;
                            notaryMessage.UserId = UserInfo.Id;
                            _iGeneralRepository.Add(notaryMessage);
                            if (await _iGeneralRepository.Save())
                            {
                                DictionaryMessage.Add("SMS ntification with id " + notaryMessage.Id, " was added successfully");
                            }
                            else
                            {
                                DictionaryMessage.Add("error message (" + _iGeneralRepository.GetNewValueBySec() + ") : ", "error when you try to insert into NotaryMessage table ");
                            }
                        }
                        else if (channelId == SmsShortCutIdInTable)
                        {

                            NotaryEmail notaryEmail = new NotaryEmail();
                            notaryEmail.Mobile = UserInfo.MobileNo;
                            notaryEmail.IsSent = 0;
                            notaryEmail.SendCount = 0;
                            notaryEmail.EmailValueId = notificationId;
                            notaryEmail.Email = UserInfo.Email;
                            notaryEmail.UserId = UserInfo.Id;
                            _iGeneralRepository.Add(notaryEmail);
                            if (await _iGeneralRepository.Save())
                            {
                                DictionaryMessage.Add("email notification with id " + notaryEmail.Id, " was added successfully");
                            }
                            else
                            {
                                DictionaryMessage.Add("error message (" + _iGeneralRepository.GetNewValueBySec() + ") : ", "error when you try to insert into NotaryEmail table ");

                            }
                        }
                    }
                    else
                    {
                        DictionaryMessage.Add("error message (" + _iGeneralRepository.GetNewValueBySec() + ") : ", "error when you try to insert into notification table ");
                    }
                }

            }

            return DictionaryMessage;
        }


        public List<notification_detials_for_action> GetAutoNotification(int ActionId)
        {
            int? notification_type_id = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == "NotificationTemplate_NotificationSendingTypeId1").Select(x => x.Id).FirstOrDefault();

            List<notification_detials_for_action> not = GetNotification(ActionId).Result.Where(x => x.notification_type_id == notification_type_id).ToList();


            return not;
        }

        public List<notification_detials_for_action> GetNotificationToSendByEmp(int ActionId)
        {
            int? notification_type_id = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == "NotificationTemplate_NotificationSendingTypeId2").Select(x => x.Id).FirstOrDefault();

            List<notification_detials_for_action> not = GetNotification(ActionId).Result.Where(x => x.notification_type_id == notification_type_id).ToList();


            return not;
        }


        public Task<List<notification_detials_for_action>> GetNotification(int ActionId)
        {

            List<notification_detials_for_action> not = (from Notification_Action in _EngineCoreDBContext.NotificationAction.Where(x => x.ActionId == ActionId)

                                                             // from Notification_templete in _EngineCoreDBContext.NotificationTemplate.Where(y => y.Id == Notification_Action.NotificationDetialsId && y.NotificationSendingTypeId == notification_type_id)
                                                         from Notification_templete in _EngineCoreDBContext.NotificationTemplate.Where(y => y.Id == Notification_Action.NotificationTemplateId)
                                                         select new notification_detials_for_action
                                                         {

                                                             notification_templete_id = Notification_Action.NotificationTemplateId,
                                                             notification_templete_shortcut = Notification_templete.NotificationNameShortcut,


                                                             //       notification_type_id = _EngineCoreDBContext.NotificationTemplate.Where(z => z.Id == Notification_Action.NotificationTemplateId).Select(n => n.NotificationSendingTypeId).FirstOrDefault(),


                                                             notification_templete_translation = (from TranslationTB in _EngineCoreDBContext.SysTranslation.Where(z => z.Shortcut == Notification_templete.NotificationNameShortcut)
                                                                                                  select new MessageDictionary
                                                                                                  {
                                                                                                      lang = TranslationTB.Lang,
                                                                                                      shortcut_translation = TranslationTB.Value
                                                                                                  }).ToList(),
                                                             NotificationDetials = (from Notification_detials in _EngineCoreDBContext.NotificationTemplateDetail.Where(y => y.NotificationTemplateId == Notification_templete.Id)
                                                                                    select new NotificationDetialsForSendingProcess
                                                                                    {
                                                                                        notification_details_id = Notification_detials.Id,
                                                                                        channel_id = Notification_detials.NotificationChannelId,
                                                                                        body_shortcut = Notification_detials.BodyShortcut,
                                                                                        title_shortcut = Notification_detials.TitleShortcut,
                                                                                        channel_shortcut = _EngineCoreDBContext.SysLookupValue.Where(o => o.Id == Notification_detials.NotificationChannelId).Select(p => p.Shortcut).FirstOrDefault(),
                                                                                        title_shortcut_translation = (from TranslationTB in _EngineCoreDBContext.SysTranslation.Where(z => z.Shortcut == Notification_detials.TitleShortcut)
                                                                                                                      select new MessageDictionary
                                                                                                                      {
                                                                                                                          lang = TranslationTB.Lang,
                                                                                                                          shortcut_translation = TranslationTB.Value
                                                                                                                      }).ToList(),
                                                                                        body_shortcut_translation = (from TranslationTB in _EngineCoreDBContext.SysTranslation.Where(z => z.Shortcut == Notification_detials.BodyShortcut)
                                                                                                                     select new MessageDictionary
                                                                                                                     {
                                                                                                                         lang = TranslationTB.Lang,
                                                                                                                         shortcut_translation = TranslationTB.Value
                                                                                                                     }).ToList(),
                                                                                        channel_name_translation = (from TranslationTB in _EngineCoreDBContext.SysTranslation.Where(z => z.Shortcut == _EngineCoreDBContext.SysLookupValue.Where(o => o.Id == Notification_detials.NotificationChannelId).Select(p => p.Shortcut).FirstOrDefault())
                                                                                                                    select new MessageDictionary
                                                                                                                    {
                                                                                                                        lang = TranslationTB.Lang,
                                                                                                                        shortcut_translation = TranslationTB.Value
                                                                                                                    }).ToList()


                                                                                    }
                                                 ).ToList()


                                                         }).ToList();

            return Task.FromResult(not);

        }

        public class AcuallNotification
        {
            [Required]
            public int senderId { get; set; }
            public List<AcualNotificationReciverInfo> Notifications { get; set; }
        }
        public class AcualNotificationReciverInfo
        {
            [Required]
            public int reciverId { get; set; }
            public List<AcualNotificationMessageInfo> NotificationMessageInfo { get; set; }
        }
        public class AcualNotificationMessageInfo
        {
            //[Required]
            public string title { get; set; }
            [Required]
            public string body { get; set; }
            [Required]
            public int? channel_id { get; set; }
            [Required]
            public string lang { get; set; }//notificationTemplateId
            [Required]
            public int? notificationTemplateId { get; set; }//notificationTemplateId
        }




    }


}

*/