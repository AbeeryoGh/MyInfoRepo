using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ApplicationPartyExtraAttachment
    {
        public int Id { get; set; }
        public int ApplicationPartyId { get; set; }
        public int? AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentUrl { get; set; }
        public string Number { get; set; }
        public int? CountryOfIssue { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }

        public virtual ApplicationParty ApplicationParty { get; set; }
        public virtual SysLookupValue Attachment { get; set; }
    }
}
