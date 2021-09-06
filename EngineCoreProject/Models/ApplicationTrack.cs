using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ApplicationTrack
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int? UserId { get; set; }
        public int? StageId { get; set; }
        public int? NextStageId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }
        public string Note { get; set; }
        public int? LocationId { get; set; }
        public short? NoteKind { get; set; }

        public virtual Application Application { get; set; }
        public virtual Location Location { get; set; }
        public virtual User User { get; set; }
    }
}
