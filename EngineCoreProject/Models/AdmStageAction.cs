using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AdmStageAction
    {
        public int Id { get; set; }
        public int? StageId { get; set; }
        public int? ActionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public byte? ShowOrder { get; set; }
        public bool? Enabled { get; set; }
        public string Group { get; set; }

        public virtual AdmAction Action { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
        public virtual AdmStage Stage { get; set; }
    }
}
