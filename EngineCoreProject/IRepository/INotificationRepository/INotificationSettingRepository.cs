using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.INotificationSettingRepository
{
    public interface INotificationSettingRepository
    {
        Task<int> AddNotificationTemplateWithDetails(NotificationTemplateWithDetailsPostDto notificationTemplatePostDto, string lang);
        Task<List<int>> AddNotificationAction(NotificationActionPostDto notificationActionPostDto, int notificationTemplateId);
        Task<int> AddNotificationTemplatesToOneAction(NotificationTemplatesActionPostDto notificationTemplatesAction);
        Task<bool> DeleteNotificationTemplate(int id);
        Task<List<NotificationTemplateGetDto>> GetAllNotificationTemplates();
        Task<NotificationTemplateWithDetailsGetDto> GetAllNotificationDetails(int notifyTemplateId);
        Task<int> EditNotificationTemplateDetials(NotificationTemplateWithDetailsPostDto notificationTemplateDetails, int templateId);
        Dictionary<string, string> GetParameterList();
        Task <List<NotificationLogPostDto>> GetNotificationsForAction(int actionId);

        Task GetAllNotificationDetailsLans(string lang);

    }
}