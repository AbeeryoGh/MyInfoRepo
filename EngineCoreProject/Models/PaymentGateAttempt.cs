using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class PaymentGateAttempt
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public DateTime PaidAttemptDate { get; set; }
        public string SecureHash { get; set; }
        public string EDirhamFee { get; set; }
        public string EServiceData { get; set; }
        public string ConfirmationId { get; set; }
        public string Pun { get; set; }
        public string CollectionCenterFee { get; set; }
        public string PaymentAttemptInvoiceNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
