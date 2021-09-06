using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService.ModelView
{
    public class StageOfService
    {
            public int    Id { get; set; }
            public int    StageTypeId { get; set; }
            public int    Order { get; set; }
            public string Name { get; set; }
            public bool   IsCurrent { get; set; }
            public string Icon { get; set; }


    }
}
