using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AppTransaction
    {
        public AppTransaction()
        {
            ApplicationParty = new HashSet<ApplicationParty>();
            TransactionOldVersion = new HashSet<TransactionOldVersion>();
        }

        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }
        public string Content { get; set; }
        public string TransactionNo { get; set; }
        public string DocumentUrl { get; set; }
        public string DecisionText { get; set; }
        public DateTime? TransactionStartDate { get; set; }
        public DateTime? TransactionEndDate { get; set; }
        public bool? UnlimitedValidity { get; set; }
        public int? TransactionStatus { get; set; }
        public DateTime? TransactionCreatedDate { get; set; }
        public string Qrcode { get; set; }
        public string Title { get; set; }
        public double? ContractValue { get; set; }
        public string TransactionRefNo { get; set; }
        public string Vcid { get; set; }
        public bool? Exist { get; set; }
        public string RevocationReason { get; set; }

        public virtual Application Application { get; set; }
        public virtual ICollection<ApplicationParty> ApplicationParty { get; set; }
        public virtual ICollection<TransactionOldVersion> TransactionOldVersion { get; set; }
    }
}
