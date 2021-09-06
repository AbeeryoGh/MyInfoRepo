using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.BasherDto
{

    public class retrieveNotaryFees_MOJResponse
    {
        public string EODBTrackingNumber { get; set; }
        public double totalFeesAmount   { get; set; }
        public List<FeesBreakDown> FeeDetailsList { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }

        public retrieveNotaryFees_MOJResponse()
        {
            FeeDetailsList = new List<FeesBreakDown>();
        }
    }

    public class RetrieveNotaryFeesMOJResponseError
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }


}
