using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AdmStage
    {
        public AdmStage()
        {
            AdmStageAction = new HashSet<AdmStageAction>();
            Application = new HashSet<Application>();
            StageMasterAttachment = new HashSet<StageMasterAttachment>();
        }

        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public string Shortcut { get; set; }
        public int? PeriodForArchive { get; set; }
        public int? PeriodForLate { get; set; }
        public int? OrderNo { get; set; }
        public int? Fee { get; set; }
        public int? StageTypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }
        public string Icon { get; set; }
        public string KhadamatiServiceNo { get; set; }

        public virtual AdmService Service { get; set; }
        public virtual SysLookupValue StageType { get; set; }
        public virtual ICollection<AdmStageAction> AdmStageAction { get; set; }
        public virtual ICollection<Application> Application { get; set; }
        public virtual ICollection<StageMasterAttachment> StageMasterAttachment { get; set; }
    }
}
