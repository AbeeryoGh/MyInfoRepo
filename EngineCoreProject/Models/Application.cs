using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Application
    {
        public Application()
        {
            AppRelatedContent = new HashSet<AppRelatedContent>();
            ApplicationAttachment = new HashSet<ApplicationAttachment>();
            ApplicationTrack = new HashSet<ApplicationTrack>();
            AramexRequests = new HashSet<AramexRequests>();
            TargetApplicationApp = new HashSet<TargetApplication>();
            TargetApplicationTargetApp = new HashSet<TargetApplication>();
        }

        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public string ApplicationNo { get; set; }
        public int? TemplateId { get; set; }
        public int? CurrentStageId { get; set; }
        public int? StateId { get; set; }
        public string Note { get; set; }
        public DateTime? AppLastUpdateDate { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public int? Channel { get; set; }
        public int? Reason { get; set; }
        public bool? Locked { get; set; }
        public int? LastReader { get; set; }
        public DateTime? LastReadDate { get; set; }
        public int? Owner { get; set; }
        public long? OldTemplateId { get; set; }
        public int? OldId { get; set; }
        public bool? Delivery { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual AdmStage CurrentStage { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
        public virtual AdmService Service { get; set; }
        public virtual Template Template { get; set; }
        public virtual AppTransaction AppTransaction { get; set; }
        public virtual ICollection<AppRelatedContent> AppRelatedContent { get; set; }
        public virtual ICollection<ApplicationAttachment> ApplicationAttachment { get; set; }
        public virtual ICollection<ApplicationTrack> ApplicationTrack { get; set; }
        public virtual ICollection<AramexRequests> AramexRequests { get; set; }
        public virtual ICollection<TargetApplication> TargetApplicationApp { get; set; }
        public virtual ICollection<TargetApplication> TargetApplicationTargetApp { get; set; }
    }
}
