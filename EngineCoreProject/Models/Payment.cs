using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Payment
    {
        public Payment()
        {
            PaymentDetails = new HashSet<PaymentDetails>();
            PaymentGateAttempt = new HashSet<PaymentGateAttempt>();
        }

        public int Id { get; set; }
        public string ReceiptNo { get; set; }
        public double? TotalAmount { get; set; }
        public string InvoiceNo { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string StatusMessage { get; set; }
        public string PaymentType { get; set; }
        public string PaymentSource { get; set; }
        public string PaymentMethodType { get; set; }
        public int? ApplicationId { get; set; }
        public int? ServiceId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public DateTime? TransactionResponseDate { get; set; }
        public int? ActionId { get; set; }
        public bool? Printed { get; set; }
        public double? ActualPaid { get; set; }

        public virtual AdmService Service { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<PaymentDetails> PaymentDetails { get; set; }
        public virtual ICollection<PaymentGateAttempt> PaymentGateAttempt { get; set; }
    }
}
