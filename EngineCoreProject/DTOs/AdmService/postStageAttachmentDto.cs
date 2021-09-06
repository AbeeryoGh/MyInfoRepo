using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class postStageAttachmentDto
    {
        public int? stageId { get; set; }
        public int? attachId { get; set; }
      
        public bool? required { get; set; }

    }
}
