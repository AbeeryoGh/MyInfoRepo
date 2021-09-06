using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ApplicationParty
    {
        public ApplicationParty()
        {
            ApplicationPartyExtraAttachment = new HashSet<ApplicationPartyExtraAttachment>();
        }

        public int Id { get; set; }
        public int? PartyId { get; set; }
        public int? TransactionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public bool? IsOwner { get; set; }
        public int? PartyTypeValueId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? MaritalStatus { get; set; }
        public int? Gender { get; set; }
        public string EmiratesIdNo { get; set; }
        public DateTime? IdExpirationDate { get; set; }
        public string IdAttachment { get; set; }
        public string UnifiedNumber { get; set; }
        public bool? SignRequired { get; set; }
        public bool? Signed { get; set; }
        public int? SignType { get; set; }
        public DateTime? SignDate { get; set; }
        public string SignUrl { get; set; }
        public bool? EditableSign { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int? Emirate { get; set; }
        public string City { get; set; }
        public string AlternativeEmail { get; set; }
        public int? NotaryId { get; set; }

        public virtual User Party { get; set; }
        public virtual SysLookupValue PartyTypeValue { get; set; }
        public virtual AppTransaction Transaction { get; set; }
        public virtual ICollection<ApplicationPartyExtraAttachment> ApplicationPartyExtraAttachment { get; set; }
    }
}
