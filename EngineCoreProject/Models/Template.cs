using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Template
    {
        public Template()
        {
            Application = new HashSet<Application>();
            RelatedContent = new HashSet<RelatedContent>();
            RelatedData = new HashSet<RelatedData>();
            TemplateAttachment = new HashSet<TemplateAttachment>();
            TemplateParty = new HashSet<TemplateParty>();
            Term = new HashSet<Term>();
        }

        public int Id { get; set; }
        public int? DocumentTypeId { get; set; }
        public string TitleShortcut { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public byte? Type { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual SysLookupValue DocumentType { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
        public virtual ICollection<Application> Application { get; set; }
        public virtual ICollection<RelatedContent> RelatedContent { get; set; }
        public virtual ICollection<RelatedData> RelatedData { get; set; }
        public virtual ICollection<TemplateAttachment> TemplateAttachment { get; set; }
        public virtual ICollection<TemplateParty> TemplateParty { get; set; }
        public virtual ICollection<Term> Term { get; set; }
    }
}
