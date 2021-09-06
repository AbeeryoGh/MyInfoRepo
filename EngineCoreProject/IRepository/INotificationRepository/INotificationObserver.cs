
using EngineCoreProject.DTOs.NotificationDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace EngineCoreProject.IRepository.INotificationSettingRepository
{
    interface INotificationObserver
    {
       Task<List<NotificationLogPostDto>> Notify(bool sendImmediately);
    }
}
