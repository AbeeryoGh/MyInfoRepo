using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class AppServiceStage
    {
        public int AppId { get; set; }
        public string ServiceName { get; set; }
        public string StageName { get; set; }
        public int App_state { get; set; }
        public bool? Block_Target { get; set; }
    }
}
