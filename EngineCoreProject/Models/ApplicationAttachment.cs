using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ApplicationAttachment
    {
        public int Id { get; set; }
        public string MimeType { get; set; }
        public long? Size { get; set; }
        public int? ApplicationId { get; set; }
        public int? AttachmentId { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public string Note { get; set; }
        public long? OldAttachId { get; set; }
        public int? OldId { get; set; }

        public virtual Application Application { get; set; }
        public virtual SysLookupValue Attachment { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
    }
}
