using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class NotificationTemplate
    {
        public NotificationTemplate()
        {
            NotificationAction = new HashSet<NotificationAction>();
            NotificationTemplateDetail = new HashSet<NotificationTemplateDetail>();
        }

        public int Id { get; set; }
        public string NotificationNameShortcut { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }

        public virtual ICollection<NotificationAction> NotificationAction { get; set; }
        public virtual ICollection<NotificationTemplateDetail> NotificationTemplateDetail { get; set; }
    }
}
