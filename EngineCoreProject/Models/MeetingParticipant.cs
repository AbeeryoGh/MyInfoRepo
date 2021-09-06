using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class MeetingParticipant
    {
        public int MeetingId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }

        public virtual Meeting Meeting { get; set; }
        public virtual User User { get; set; }
    }
}
