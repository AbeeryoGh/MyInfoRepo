using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class NotificationAction
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int NotificationTemplateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }

        public virtual AdmAction Action { get; set; }
        public virtual NotificationTemplate NotificationTemplate { get; set; }
    }
}
