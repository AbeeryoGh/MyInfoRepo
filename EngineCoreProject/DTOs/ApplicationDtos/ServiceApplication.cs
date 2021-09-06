using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ServiceApplication
    {
        public int ApplicationId { get; set; }

        public int ServiceId { get; set; }

        public int StateId { get; set; }

        public int StageTyeId {get;set;}
    }
}
