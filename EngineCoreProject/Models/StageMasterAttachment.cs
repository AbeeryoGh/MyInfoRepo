using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class StageMasterAttachment
    {
        public int Id { get; set; }
        public int? StageId { get; set; }
        public int? MasterAttachmentId { get; set; }
        public int? TemplateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool? Required { get; set; }

        public virtual SysLookupValue MasterAttachment { get; set; }
        public virtual AdmStage Stage { get; set; }
    }
}
