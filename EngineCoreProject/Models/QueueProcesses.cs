using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class QueueProcesses
    {
        public int Id { get; set; }
        public string ProcessNo { get; set; }
        public int ServiceKindNo { get; set; }
        public DateTime ExpectedDateTime { get; set; }
        public bool? NotifyLowLevel { get; set; }
        public int TicketId { get; set; }
        public string Note { get; set; }
        public string Provider { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }
        public bool? NotifyHighLevel { get; set; }
        public bool? NotifyMediumLevel { get; set; }
        public byte? Status { get; set; }

        public virtual ServiceKind ServiceKindNoNavigation { get; set; }
    }
}
