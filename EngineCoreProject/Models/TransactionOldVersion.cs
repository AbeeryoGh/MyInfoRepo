using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class TransactionOldVersion
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string TransactionNo { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime TransactionCreatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string RecStatus { get; set; }
        public string Note { get; set; }
        public int? OldTransactionId { get; set; }

        public virtual AppTransaction Transaction { get; set; }
    }
}
