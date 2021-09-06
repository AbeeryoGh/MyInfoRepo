using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class StageActionDto
    {
        public int? recId { get; set; }
        public int? stageId { get; set; }
        public int? actionId { get; set; }
        public string ActionName { get; set; }
        public int? actiontypeid { get; set; }
        public string lang { get; set; }
        public string value { get; set; }
        public string actiontype { get; set; }
        public string typename { get; set; }

    }
}
