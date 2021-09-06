using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ServiceFee
    {
        public int Id { get; set; }
        public int ServiceNo { get; set; }
        public int FeeNo { get; set; }
        public int DocumentKind { get; set; }
        public int ProcessKind { get; set; }
        public bool? Required { get; set; }

        public virtual TransactionFee FeeNoNavigation { get; set; }
        public virtual AdmService ServiceNoNavigation { get; set; }
    }
}
