using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class ManualPaymentDetialsDto
    {
        public int AppId { set; get; }
        public int UserId { set; get; }
        public string ReceiptNo { set; get; }
        public DateTime PayDate { set; get; }
        public double TotalAmount { set; get; }
    }


    public class ResManualPayDto
    {  

        public string Status { set; get; }
        public int? ServiceId { set; get; }
    }

    public class ApplicationID
    {
        public int AppId { set; get; }
    }

}
