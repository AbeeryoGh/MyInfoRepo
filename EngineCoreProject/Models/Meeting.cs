using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Meeting
    {
        public Meeting()
        {
            Calendar = new HashSet<Calendar>();
            MeetingLogging = new HashSet<MeetingLogging>();
        }

        public int Id { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TimeZone { get; set; }
        public string Password { get; set; }
        public bool? PasswordReq { get; set; }
        public string MeetingId { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public string OrderNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }
        public string MeetingLog { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Calendar> Calendar { get; set; }
        public virtual ICollection<MeetingLogging> MeetingLogging { get; set; }
    }
}
