using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationLogGetDto
    {
        public int Id { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationBody { get; set; }
        public int NotificationChannelId { get; set; }
        public string SendReportId { get; set; }
        public byte? IsSent { get; set; }
        public byte SentCount { get; set; }
        public string ReportValueId { get; set; }
        public int? UserId { get; set; }
        public string ToAddress { get; set; }
        public string Lang { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ApplicationId { get; set; }



        public static NotificationLogGetDto GetDTO(NotificationLog notificationLog)
        {
            NotificationLogGetDto dto = new NotificationLogGetDto
            {
                Id = notificationLog.Id,
                IsSent = notificationLog.IsSent,
                Lang = notificationLog.Lang,
                NotificationBody = notificationLog.NotificationBody,
                NotificationChannelId = notificationLog.NotificationChannelId,
                NotificationTitle = notificationLog.NotificationTitle,
                ReportValueId = notificationLog.ReportValueId,
                SendReportId = notificationLog.SendReportId,
                SentCount = notificationLog.SentCount,
                ToAddress = notificationLog.ToAddress,
                ApplicationId = notificationLog.ApplicationId,
                UserId = notificationLog.UserId,
                CreatedAt = notificationLog.CreatedAt,
                UpdatedAt = notificationLog.UpdatedAt
            };

            return dto;
        }

    }






    public class NotificationLogForApp
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Channel { get; set; }
        public string SendReportId { get; set; }
        public string IsSent { get; set; }
        public byte SentCount { get; set; }
        public string ReportValueId { get; set; }
        public int? UserId { get; set; }
        public string ToAddress { get; set; }
        public string Lang { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ApplicationId { get; set; }
    }



}
