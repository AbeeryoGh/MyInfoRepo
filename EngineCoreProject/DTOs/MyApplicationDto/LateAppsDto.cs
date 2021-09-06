using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.MyApplicationDto
{
    public class LateAppsDto
    {
        public DateTime LastUpdateDate { get; set; }
        public int StateId { get; set; }
        public int ApplicationId { get; set; }
        public int ServiceId { get; set; }
        public int StageTypeId { get; set; }
        public string ApplicationNo { get; set; }
        public string ServiceName { get; set; }
        public string TemplateName { get; set; }
        public string stageName { get; set; }
        public bool islate { get; set; }
    }
}
