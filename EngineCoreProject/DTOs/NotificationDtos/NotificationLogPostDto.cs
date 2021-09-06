using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationLogPostDto
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
        public string HostSetting { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ApplicationId { get; set; }

        public NotificationLogPostDto()
        {
            CreatedDate = DateTime.Now;
        }
        public NotificationLog GetEntity()
        {
            NotificationLog notifyLog = new NotificationLog
            {
                Id = Id,
                NotificationTitle = NotificationTitle,
                NotificationBody = NotificationBody,
                IsSent = IsSent,
                Lang = Lang,
                NotificationChannelId = NotificationChannelId,
                ReportValueId = ReportValueId,
                SendReportId = SendReportId,
                SentCount = SentCount,
                ToAddress = ToAddress,
                UserId = UserId,
                Hostsetting = HostSetting,
                CreatedAt = CreatedDate,
                UpdatedAt = UpdatedDate,
                ApplicationId = ApplicationId
            };

            return notifyLog;
        }

        static public NotificationLogPostDto GetDto(NotificationLog notifyLog)
        {
            NotificationLogPostDto dto = new NotificationLogPostDto
            {
                Id = notifyLog.Id,
                HostSetting = notifyLog.Hostsetting,
                IsSent = notifyLog.IsSent,
                Lang = notifyLog.Lang,
                NotificationBody = notifyLog.NotificationBody,
                NotificationChannelId = notifyLog.NotificationChannelId,
                NotificationTitle = notifyLog.NotificationTitle,
                ReportValueId = notifyLog.ReportValueId,
                SendReportId = notifyLog.SendReportId,
                SentCount = notifyLog.SentCount,
                ToAddress = notifyLog.ToAddress,
                UserId = notifyLog.UserId,
                CreatedDate = notifyLog.CreatedAt,
                UpdatedDate = notifyLog.UpdatedAt,
                ApplicationId = notifyLog.ApplicationId
            };
            return dto;
            
        }


        public NotificationLogPostDto ShallowCopy()
        {
            NotificationLogPostDto other = (NotificationLogPostDto)this.MemberwiseClone();
            return other;
        }
    }
}
