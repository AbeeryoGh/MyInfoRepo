using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationPartyWithExtraDto : ApplicationPartyDto
    {
        public ICollection<ApplicationPartyExtraAttachmentDto> PartyExtraAttachment { get; set; }
    }
}
