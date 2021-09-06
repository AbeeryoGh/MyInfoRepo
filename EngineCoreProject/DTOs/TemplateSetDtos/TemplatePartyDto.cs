using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TemplatePartyDto
    {
        public int TemplateId { get; set; }
        public int PartyId { get; set; }
        public bool? Required { get; set; }
        public bool? SignRequired { get; set; }


    }
}
