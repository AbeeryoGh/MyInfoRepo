using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class ResponseEServiceData
    {
        public string quantity { get; set; }
        public string price { get; set; }
        public string ownerFees { get; set; }
        public string amountWithFees { get; set; }
        public string amountWithoutFees { get; set; }
        public string mainSubCode { get; set; }
    }
}
