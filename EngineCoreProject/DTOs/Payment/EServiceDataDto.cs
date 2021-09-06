using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{

    public class EServiceDataDto
    {
        public EServiceSubData[] EServiceSubDataArr { get; set; }
    }

    public class EServiceSubData
    {
        public string quantity { get; set; }
        public string price { get; set; }
        public string ownerFees { get; set; }
        public string amountWithFees { get; set; }
        public string amountWithoutFees { get; set; }
        public string mainSubCode { get; set; }
    }

    public class EServiceSubDataAfterOrder
    {
        public string[] ownerFees { get; set; }
        public string[] mainSubCode { get; set; }
       
        public string[] price { get; set; }
      
        public int[] amountWithFees { get; set; }
        public string[] quantity { get; set; }
        public int[] amountWithoutFees { get; set; }
       
    }

}
