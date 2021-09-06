using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.INotificationSettingRepository
{
    public interface INotificationLogRepository
    {
        Task<int> AddNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto);
        Task<int> UpdateNotificationsLog(List<NotificationLogPostDto> notificationsLogPostDto);
        Task<List<NotificationLogGetDto>> GetInternalNotificationsLog(int userId, string lang);
        Task<bool> ReadInternalNotificationsLog(int userId, int notifyId);
        Task<int> UpdateInternalNotificationsLogState(int notificationID);
        Task<List<NotificationLogForApp>> GetApplicationNotificationsLog(int applicationId, string lang);
    }
}
