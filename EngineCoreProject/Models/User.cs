using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {
            AdmServiceCreatedByNavigation = new HashSet<AdmService>();
            AdmServiceLastUpdatedByNavigation = new HashSet<AdmService>();
            AdmStageActionCreatedByNavigation = new HashSet<AdmStageAction>();
            AdmStageActionLastUpdatedByNavigation = new HashSet<AdmStageAction>();
            ApplicationAttachmentCreatedByNavigation = new HashSet<ApplicationAttachment>();
            ApplicationAttachmentLastUpdatedByNavigation = new HashSet<ApplicationAttachment>();
            ApplicationCreatedByNavigation = new HashSet<Application>();
            ApplicationLastUpdatedByNavigation = new HashSet<Application>();
            ApplicationParty = new HashSet<ApplicationParty>();
            ApplicationTrack = new HashSet<ApplicationTrack>();
            Calendar = new HashSet<Calendar>();
            DocumentStorage = new HashSet<DocumentStorage>();
            EmployeeSetting = new HashSet<EmployeeSetting>();
            FileConfigurationCreatedByNavigation = new HashSet<FileConfiguration>();
            FileConfigurationLastUpdatedByNavigation = new HashSet<FileConfiguration>();
            Meeting = new HashSet<Meeting>();
            MeetingLogging = new HashSet<MeetingLogging>();
            OtpLog = new HashSet<OtpLog>();
            Payment = new HashSet<Payment>();
            TemplateCreatedByNavigation = new HashSet<Template>();
            TemplateLastUpdatedByNavigation = new HashSet<Template>();
            TermCreatedByNavigation = new HashSet<Term>();
            TermLastUpdatedByNavigation = new HashSet<Term>();
            UserClaim = new HashSet<UserClaim>();
            UserLogger = new HashSet<UserLogger>();
            UserLogin = new HashSet<UserLogin>();
            UserRole = new HashSet<UserRole>();
            UserToken = new HashSet<UserToken>();
        }

        public string FullName { get; set; }
        public int? SecurityQuestionId { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Gender { get; set; }
        public int? NatId { get; set; }
        public string TelNo { get; set; }
        public int? Status { get; set; }
        public string EmailLang { get; set; }
        public string SmsLang { get; set; }
        public int? AreaId { get; set; }
        public int? NotificationType { get; set; }
        public int? ProfileStatus { get; set; }
        public string Address { get; set; }
        public string EmiratesId { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public string Sign { get; set; }
        public string EmaritIdOldUsers { get; set; }
        public string EmailIdOldUsers { get; set; }
        public int? NotaryPlaceId { get; set; }
        public int? LocationId { get; set; }

        public virtual ICollection<AdmService> AdmServiceCreatedByNavigation { get; set; }
        public virtual ICollection<AdmService> AdmServiceLastUpdatedByNavigation { get; set; }
        public virtual ICollection<AdmStageAction> AdmStageActionCreatedByNavigation { get; set; }
        public virtual ICollection<AdmStageAction> AdmStageActionLastUpdatedByNavigation { get; set; }
        public virtual ICollection<ApplicationAttachment> ApplicationAttachmentCreatedByNavigation { get; set; }
        public virtual ICollection<ApplicationAttachment> ApplicationAttachmentLastUpdatedByNavigation { get; set; }
        public virtual ICollection<Application> ApplicationCreatedByNavigation { get; set; }
        public virtual ICollection<Application> ApplicationLastUpdatedByNavigation { get; set; }
        public virtual ICollection<ApplicationParty> ApplicationParty { get; set; }
        public virtual ICollection<ApplicationTrack> ApplicationTrack { get; set; }
        public virtual ICollection<Calendar> Calendar { get; set; }
        public virtual ICollection<DocumentStorage> DocumentStorage { get; set; }
        public virtual ICollection<EmployeeSetting> EmployeeSetting { get; set; }
        public virtual ICollection<FileConfiguration> FileConfigurationCreatedByNavigation { get; set; }
        public virtual ICollection<FileConfiguration> FileConfigurationLastUpdatedByNavigation { get; set; }
        public virtual ICollection<Meeting> Meeting { get; set; }
        public virtual ICollection<MeetingLogging> MeetingLogging { get; set; }
        public virtual ICollection<OtpLog> OtpLog { get; set; }
        public virtual ICollection<Payment> Payment { get; set; }
        public virtual ICollection<Template> TemplateCreatedByNavigation { get; set; }
        public virtual ICollection<Template> TemplateLastUpdatedByNavigation { get; set; }
        public virtual ICollection<Term> TermCreatedByNavigation { get; set; }
        public virtual ICollection<Term> TermLastUpdatedByNavigation { get; set; }
        public virtual ICollection<UserClaim> UserClaim { get; set; }
        public virtual ICollection<UserLogger> UserLogger { get; set; }
        public virtual ICollection<UserLogin> UserLogin { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<UserToken> UserToken { get; set; }
    }
}
