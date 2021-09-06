using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Statistics
{
    public class searchChartsDto
    {
        public string serviceId { get; set; }
        public string appStateId { get; set; }
        public string appChannelId { get; set; }

        public string dateKind { get; set; }
        public string stageTypeId { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }
        
    }
}