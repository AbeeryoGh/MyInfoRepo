using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.NotificationDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.INotificationSettingRepository
{
    public interface ISendNotificationRepository
    {
        /// <summary>
        /// Send the notifications by the observer.
        /// </summary>
        /// <param name="notifications">list of notifications to send</param>
        /// <param name="addOrUpdateNotificationslog">true to add the notifications to the log which are not send before, false if existed before to update</param>
        /// <returns></returns>
        Task DoSend(List<NotificationLogPostDto> notifications, bool sendImmediately, bool addOrUpdateNotificationslog = true);

        /// <summary>
        /// Re send the failed notification (it is status is error),
        /// attempt every notification to resend until MAX_NOTIFY_SEND_ATTEMPTS
        /// </summary>
        /// <returns></returns>
        Task ReSend();
        Task<LogInResultDto> VerifyOTP(int userId, string number, string lang);
        Task<string> GenerateUrlToken(int userId, int serviceId, int applicationId, string lang);
        Task<UserAppDto> VerifyToken(Guid guid, string lang);
        Task NotifyQueue();
    }
}
