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
using EngineCoreProject.DTOs.ActionDtos;
using EngineCoreProject.DTOs.AdmService;
using System.Collections;

namespace EngineCoreProject.Services.NotificationService
{
    class ControlNotification
    {
        public List<INotificationObserver> Notifications = new List<INotificationObserver>();

        public List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
        /// <summary>  
        /// Add object of notification System  
        /// </summary>  
        /// <param name="obj">Object is notification class</param>  
        public void AddService(INotificationObserver obj)
        {
            Notifications.Add(obj);
        }

        /// <summary>  
        /// Remove object of notification System  
        /// </summary>  
        /// <param name="obj">Object of notification Class</param>  
        public void RemoveService(INotificationObserver obj)
        {
            Notifications.Remove(obj);
        }
        public List<NotificationLogPostDto> ExecuteNotifier(bool sendImmediately)
        {
            foreach (INotificationObserver O in Notifications)
            {
                //Call all notification System  
                res.AddRange(O.Notify(sendImmediately).Result);
            }

            return  res;
        }
    }
}
