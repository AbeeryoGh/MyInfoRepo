using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Calendar
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public int? NotifyMe { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime EndDate { get; set; }
        public int? MeetingId { get; set; }
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
