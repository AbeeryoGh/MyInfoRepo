using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService.ModelView
{
    public class ActionForRole
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int StageId { get; set; }
        public string StageName { get; set; }
        public string ServiceName { get; set; }
        public bool HasAccess { get; set; }
    }
}
