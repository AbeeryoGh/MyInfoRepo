using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService.ModelView
{
    public class ActionOfStage
    {
        public int    Id { get; set; }
        public int    StageId { get; set; }
        public int    ActionId { get; set; }
        public string ActionName { get; set; }
        public byte?  Order { get; set; }
        public bool?  Enabled { get; set; }
        public string Group { get; set; }
        public ICollection<SysExecution> executions { get; set; }

    }
}
