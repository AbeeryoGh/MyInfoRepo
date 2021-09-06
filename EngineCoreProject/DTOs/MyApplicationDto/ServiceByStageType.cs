using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.MyApplicationDto
{
    public class ServiceByStageType
    {
        public int? serviceid { get; set; }
        public int? stagetypeid { get; set; }
        public int appId { get; set; }
        public DateTime? transenddate { get; set; }
        public DateTime? transstartdate { get; set; }
        public int? currentstageid { get; set; }
        public string serviceshortcut { get; set; }
        public string servicename { get; set; }

    }
}
