using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class TargetApplicationDto
    {
        public int? AppId { get; set; }
        public int? TargetAppId { get; set; }
        public string TargetAppDesc { get; set; }
        public string TargetAppDocument { get; set; }
    }
}
