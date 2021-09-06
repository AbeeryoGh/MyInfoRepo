using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationTrackDto
    {
        public int? ApplicationId { get; set; }
        public int UserId { get; set; }
        public int? StageId { get; set; }
        public int? NextStageId { get; set; }
        public string Note { get; set; }
        public short? NoteKind { get; set; }
        
      

    }
}
