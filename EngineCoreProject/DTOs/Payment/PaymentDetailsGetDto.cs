using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class PaymentDetailsGetDto
    {
        public int Id { get; set; }
        public int? PaymentId { get; set; }
        public string ServiceMainCode { get; set; }
        public string ServiceSubCode { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? AmountWithFees { get; set; }
        public double? AmountWithoutFees { get; set; }
        public double? OwnerFees { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
    }

    public class PaymentGateAttemptsGet
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
    }

    public class PaymentDetailsGetWithAttempts
    {
        public List<PaymentDetailsGetDto> PaymentDetailsGet { get; set; }
        public List<PaymentGateAttemptsGet> PaymentGateAttempts { get; set; }

    }
}
