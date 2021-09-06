using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using EngineCoreProject.IRepository.IUserRepository;

namespace EngineCoreProject.Services.NotificationService
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IUserRepository _IUserRepository;
        ValidatorException _exception;

        public NotificationLogRepository(EngineCoreDBContext EngineCoreDBContext, IUserRepository IUserRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IUserRepository = IUserRepository;
            _exception = new ValidatorException();
        }

        public async Task<int> AddNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto)
        {
            // TODO validation for notification.
            List<NotificationLog> notifyLog = new List<NotificationLog>();
            foreach (var notify in notificationsLogPostDto)
            {
                notifyLog.Add(notify.GetEntity());
            }
            _EngineCoreDBContext.NotificationLog.AddRange(notifyLog);

            return await _EngineCoreDBContext.SaveChangesAsync();
        }

        public async Task<int> UpdateNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto)
        {
            List<NotificationLog> notifyLog = new List<NotificationLog>();
            foreach (var notify in notificationsLogPostDto)
            {
                var updatedNotify = await _EngineCoreDBContext.NotificationLog.Where(x => x.Id == notify.Id).FirstOrDefaultAsync();
                if (updatedNotify != null)
                {
                    updatedNotify.IsSent = notify.IsSent;
                    updatedNotify.SendReportId = notify.SendReportId;
                    updatedNotify.Hostsetting = notify.HostSetting;
                    updatedNotify.SentCount = notify.SentCount;
                    updatedNotify.UpdatedAt = DateTime.Now;
                    updatedNotify.ToAddress = notify.ToAddress;
                    updatedNotify.ApplicationId = notify.ApplicationId;
                    updatedNotify.Lang = notify.Lang;
                    updatedNotify.NotificationBody = notify.NotificationBody;
                    updatedNotify.NotificationTitle = notify.NotificationTitle;
                }
                notifyLog.Add(updatedNotify);
            }

            _EngineCoreDBContext.NotificationLog.UpdateRange(notifyLog);

            return await _EngineCoreDBContext.SaveChangesAsync();
        }

        public async Task<List<NotificationLogGetDto>> GetInternalNotificationsLog(int userId, string lang)
        {
            List<NotificationLogGetDto> res = new List<NotificationLogGetDto>();

            int internalChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            var internalNotifications = await _EngineCoreDBContext.NotificationLog.Where(x => x.ToAddress.Trim() == userId.ToString() && x.NotificationChannelId == internalChannel).ToListAsync();
            foreach (var notify in internalNotifications)
            {
                res.Add(NotificationLogGetDto.GetDTO(notify));
            }

            return res;
        }

        public async Task<bool> ReadInternalNotificationsLog(int userId, int notifyId)
        {
            var internalNotifications = await _EngineCoreDBContext.NotificationLog.Where(x => x.Id == notifyId && x.ToAddress == userId.ToString()).FirstOrDefaultAsync();
            if (internalNotifications != null)
            {
                internalNotifications.IsSent = 1;
                internalNotifications.UpdatedAt = DateTime.Now;
                _EngineCoreDBContext.NotificationLog.Update(internalNotifications);
                await _EngineCoreDBContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<NotificationLogForApp>> GetApplicationNotificationsLog(int applicationId, string lang)
        {
            var app = await _EngineCoreDBContext.Application.Where(x => x.Id == applicationId).Include(z => z.ApplicationTrack).FirstOrDefaultAsync();
            if (app == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound"));
                throw _exception;
            }

            if (!_IUserRepository.IsAdmin())
            {
                var userId = _IUserRepository.GetUserID();
                if (!app.ApplicationTrack.Any(x => x.UserId == userId))
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "unauthoraizedForAPP"));
                    throw _exception;
                }

            }

            var sendString = (lang == "en") ? "Send" : "مرسل";
            var notSendString = (lang == "en") ? "Not Send" : "غير مرسل";
            var internalChannelName = (lang == "en") ? "Internal" : "داخلي";
            var emailChannelName = (lang == "en") ? "Email" : "ايميل";
            var smsChannelName = (lang == "en") ? "SMS" : "رسالة";

            int internalChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            int emailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            int smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            Dictionary<int, string> channels = new Dictionary<int, string>
            {
                { internalChannel, internalChannelName},
                 { emailChannel, emailChannelName},
                  { smsChannel, smsChannelName},
            };

            List<NotificationLogForApp> res = new List<NotificationLogForApp>();
            res = await (from notificationLog in _EngineCoreDBContext.NotificationLog
                         where notificationLog.ApplicationId == applicationId
                         orderby notificationLog.CreatedAt descending

                         select new NotificationLogForApp
                         {
                             Id = notificationLog.Id,
                             IsSent = (notificationLog.IsSent == 1) ? sendString : notSendString,
                             Lang = notificationLog.Lang,
                             Body = notificationLog.NotificationBody,
                             Channel = channels[notificationLog.NotificationChannelId],
                             Title = notificationLog.NotificationTitle,
                             ReportValueId = notificationLog.ReportValueId,
                             SendReportId = notificationLog.SendReportId,
                             SentCount = notificationLog.SentCount,
                             ToAddress = notificationLog.ToAddress,
                             ApplicationId = notificationLog.ApplicationId,
                             UserId = notificationLog.UserId,
                             CreatedAt = notificationLog.CreatedAt,
                             UpdatedAt = notificationLog.UpdatedAt

                         }).ToListAsync();


            return res;
        }

        public async Task<int> UpdateInternalNotificationsLogState(int notificationID)
        {
            // TODO Validate if Internal notification.
            var internalNotifications = await _EngineCoreDBContext.NotificationLog.Where(x => x.Id == notificationID).FirstOrDefaultAsync();

            if (internalNotifications != null)
            {
                internalNotifications.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                internalNotifications.UpdatedAt = DateTime.Now;
            }

            _EngineCoreDBContext.NotificationLog.Update(internalNotifications);
            return await _EngineCoreDBContext.SaveChangesAsync();
        }

    }
}
