using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class TransactionFee
    {
        public TransactionFee()
        {
            ServiceFee = new HashSet<ServiceFee>();
        }

        public int Id { get; set; }
        public string TransactionNameShortcut { get; set; }
        public double Value { get; set; }
        public string SubClass { get; set; }
        public string PrimeClass { get; set; }
        public bool Multiplied { get; set; }
        public bool Percentage { get; set; }
        public bool PerPage { get; set; }
        public int? MaxLimitedTax { get; set; }
        public int? MoreThan { get; set; }
        public int? LessThan { get; set; }
        public string Notes { get; set; }
        public bool LimitedValue { get; set; }
        public string EntityCodeEpos { get; set; }
        public string EntityGlCodeEpos { get; set; }
        public string ServiceCodeEpos { get; set; }
        public string ServiceGlCodeEpos { get; set; }
        public string MappingFmisCodeEpos { get; set; }

        public virtual ICollection<ServiceFee> ServiceFee { get; set; }
    }
}
