using EngineCoreProject.DTOs.ApplicationDtos.IdDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.Action
{
    public class StagePayload
    {
        public ApplicationWIdDto   application { get; set; }
        public ApplicationTrackDto trackDto { get; set; }
       
    }
}
