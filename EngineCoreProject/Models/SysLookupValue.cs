using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class SysLookupValue
    {
        public SysLookupValue()
        {
            AdmAction = new HashSet<AdmAction>();
            AdmServiceDocumentType = new HashSet<AdmServiceDocumentType>();
            AdmStage = new HashSet<AdmStage>();
            ApplicationAttachment = new HashSet<ApplicationAttachment>();
            ApplicationParty = new HashSet<ApplicationParty>();
            ApplicationPartyExtraAttachment = new HashSet<ApplicationPartyExtraAttachment>();
            DocumentTypeKind = new HashSet<DocumentTypeKind>();
            NotaryPlace = new HashSet<NotaryPlace>();
            NotificationTemplateDetail = new HashSet<NotificationTemplateDetail>();
            StageMasterAttachment = new HashSet<StageMasterAttachment>();
            TargetService = new HashSet<TargetService>();
            Template = new HashSet<Template>();
            TemplateAttachment = new HashSet<TemplateAttachment>();
            TemplateParty = new HashSet<TemplateParty>();
        }

        public int Id { get; set; }
        public int? LookupTypeId { get; set; }
        public string Shortcut { get; set; }
        public bool? BoolParameter { get; set; }
        public int? Order { get; set; }

        public virtual SysLookupType LookupType { get; set; }
        public virtual ICollection<AdmAction> AdmAction { get; set; }
        public virtual ICollection<AdmServiceDocumentType> AdmServiceDocumentType { get; set; }
        public virtual ICollection<AdmStage> AdmStage { get; set; }
        public virtual ICollection<ApplicationAttachment> ApplicationAttachment { get; set; }
        public virtual ICollection<ApplicationParty> ApplicationParty { get; set; }
        public virtual ICollection<ApplicationPartyExtraAttachment> ApplicationPartyExtraAttachment { get; set; }
        public virtual ICollection<DocumentTypeKind> DocumentTypeKind { get; set; }
        public virtual ICollection<NotaryPlace> NotaryPlace { get; set; }
        public virtual ICollection<NotificationTemplateDetail> NotificationTemplateDetail { get; set; }
        public virtual ICollection<StageMasterAttachment> StageMasterAttachment { get; set; }
        public virtual ICollection<TargetService> TargetService { get; set; }
        public virtual ICollection<Template> Template { get; set; }
        public virtual ICollection<TemplateAttachment> TemplateAttachment { get; set; }
        public virtual ICollection<TemplateParty> TemplateParty { get; set; }
    }
}
