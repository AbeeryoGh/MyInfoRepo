using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class NotificationLog
    {
        public int Id { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationBody { get; set; }
        public int NotificationChannelId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string SendReportId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? IsSent { get; set; }
        public byte SentCount { get; set; }
        public string ReportValueId { get; set; }
        public int? UserId { get; set; }
        public string ToAddress { get; set; }
        public string Lang { get; set; }
        public string Hostsetting { get; set; }
        public int? ApplicationId { get; set; }
    }
}
