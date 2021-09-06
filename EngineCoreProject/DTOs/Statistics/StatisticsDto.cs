using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Statistics
{
    public class StatisticsDto
    {
        public int Allapps { get; set; }
        public int services { get; set; }
        public int party { get; set; }
        public int isowner { get; set; }
        public int payments { get; set; }
        public int onlineSign { get; set; }
        public int offlineSign { get; set; }
        public int mixerSign { get; set; }
    }
}
