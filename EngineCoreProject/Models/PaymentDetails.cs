using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class PaymentDetails
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

        public virtual Payment Payment { get; set; }
    }
}
