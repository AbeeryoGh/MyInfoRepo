using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.NotificationService
{
    class InternalNotification : INotificationObserver
    {
        private readonly List<NotificationLogPostDto> _notificationsLogPostDto;
        private readonly EngineCoreDBContext _EngineCoreDBContext;

        public InternalNotification(List<NotificationLogPostDto> notificationsLogPostDto, EngineCoreDBContext EngineCoreDBContext)
        {
            _notificationsLogPostDto = new List<NotificationLogPostDto>();
            _notificationsLogPostDto = notificationsLogPostDto;
            _EngineCoreDBContext = EngineCoreDBContext;
        }

        public async Task<List<NotificationLogPostDto>> Notify(bool notUsed)
        {
            List <NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            foreach (var notify in _notificationsLogPostDto)
            {
                var userDetails = await _EngineCoreDBContext.User.Where(x => x.Id == notify.UserId).FirstOrDefaultAsync();
                if (userDetails != null)
                {              
                    notify.HostSetting = "Internal Notification";
                    notify.SentCount = 1;
                    notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                    res.Add(notify);
                }
            }
            return res;
        }
    }
}
