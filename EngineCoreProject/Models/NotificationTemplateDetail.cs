using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class NotificationTemplateDetail
    {
        public int Id { get; set; }
        public int NotificationTemplateId { get; set; }
        public int NotificationChannelId { get; set; }
        public string TitleShortcut { get; set; }
        public string BodyShortcut { get; set; }
        public bool ChangeAble { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }

        public virtual SysLookupValue NotificationChannel { get; set; }
        public virtual NotificationTemplate NotificationTemplate { get; set; }
    }
}
