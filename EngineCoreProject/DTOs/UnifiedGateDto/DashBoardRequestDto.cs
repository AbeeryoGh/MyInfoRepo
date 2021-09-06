using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGateDto
{

    public class DashBoardRequestDto
    {
        public DtdataDashBoard[] dtData { get; set; }
    }

    public class DtdataDashBoard
    {
        public string Lang { get; set; }
        public string EmiratesId { get; set; }
        public string EmailID { get; set; }
        public string MobNo { get; set; }
        public string PortalID { get; set; }
        public string ApplicationNo { get; set; }
        public string Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }


    public class responseCardDashBoardDto
    {
        public int status { get; set; }
        public responseCardDashBoard responseCardDashBoard { get; set; }
    }

    public class responseCardDashBoard
    {
        public Dtout[] dtOut { get; set; }
    }

    public class Dtout
    {
        [JsonProperty("Under Processing")]
        public string UnderProcessing { get; set; }
        [JsonProperty("Approved")]
        public string Approved { get; set; }
        [JsonProperty("Rejected")]
        public int Rejected { get; set; }
        [JsonProperty("Total")]
        public int Total { get; set; }
    }


    public class responseTableDashBoardDto
    {
        public int status { get; set; }
        public responseTableDashBoard responseTableDashBoard { get; set; }
    }

    public class responseTableDashBoard
    {
        public List<Maindata> MainData { get; set; }

        
    }

    public class Maindata
    {
        public  string  AppSource { get; set; }
        public string ApplicationNo { get; set; }
        public string ApplicationDate { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string Center { get; set; }
        public string Service { get; set; }
        public string Amount { get; set; }
        public string PortalId { get; set; }
        public object UniqueId { get; set; }
        public string UserEmail { get; set; }
        public string UserNationalID { get; set; }
        public string UserMobileNo { get; set; }
    }

}
