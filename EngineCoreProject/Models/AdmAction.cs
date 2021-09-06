using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AdmAction
    {
        public AdmAction()
        {
            AdmStageAction = new HashSet<AdmStageAction>();
            NotificationAction = new HashSet<NotificationAction>();
            SysExecution = new HashSet<SysExecution>();
        }

        public int Id { get; set; }
        public string Shortcut { get; set; }
        public int? ActionTypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }

        public virtual SysLookupValue ActionType { get; set; }
        public virtual ICollection<AdmStageAction> AdmStageAction { get; set; }
        public virtual ICollection<NotificationAction> NotificationAction { get; set; }
        public virtual ICollection<SysExecution> SysExecution { get; set; }
    }
}
