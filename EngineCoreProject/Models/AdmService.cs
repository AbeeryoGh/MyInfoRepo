using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AdmService
    {
        public AdmService()
        {
            AdmServiceDocumentType = new HashSet<AdmServiceDocumentType>();
            AdmStage = new HashSet<AdmStage>();
            Application = new HashSet<Application>();
            Payment = new HashSet<Payment>();
            RelatedContent = new HashSet<RelatedContent>();
            RelatedData = new HashSet<RelatedData>();
            ServiceFee = new HashSet<ServiceFee>();
            TargetServiceService = new HashSet<TargetService>();
            TargetServiceTargetServiceNavigation = new HashSet<TargetService>();
        }

        public int Id { get; set; }
        public int? UgId { get; set; }
        public string Shortcut { get; set; }
        public int? Fee { get; set; }
        public string Icon { get; set; }
        public int? Order { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public int? ServiceKindNo { get; set; }
        public string ApprovalText { get; set; }
        public string CancellationText { get; set; }
        public int? TargetService { get; set; }
        public bool? Templated { get; set; }
        public int? DefaultUser { get; set; }
        public bool? HasReason { get; set; }
        public short? ServiceResult { get; set; }
        public bool? BlockTarget { get; set; }
        public bool? HasDocument { get; set; }
        public int? ApprovalStage { get; set; }
        public int? TemplateId { get; set; }
        public int? DocumentTypeId { get; set; }
        public bool? LimitedTime { get; set; }
        public string KhadamatiServiceNo { get; set; }
        public short? ExternalFileRequired { get; set; }
        public string GuidFilePathAr { get; set; }
        public string GuidFilePathEn { get; set; }
        public bool? Delivery { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
        public virtual ServiceKind ServiceKindNoNavigation { get; set; }
        public virtual ICollection<AdmServiceDocumentType> AdmServiceDocumentType { get; set; }
        public virtual ICollection<AdmStage> AdmStage { get; set; }
        public virtual ICollection<Application> Application { get; set; }
        public virtual ICollection<Payment> Payment { get; set; }
        public virtual ICollection<RelatedContent> RelatedContent { get; set; }
        public virtual ICollection<RelatedData> RelatedData { get; set; }
        public virtual ICollection<ServiceFee> ServiceFee { get; set; }
        public virtual ICollection<TargetService> TargetServiceService { get; set; }
        public virtual ICollection<TargetService> TargetServiceTargetServiceNavigation { get; set; }
    }
}
