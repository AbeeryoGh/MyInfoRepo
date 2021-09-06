using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.IdDtos
{
    public class ApplicationPartyWExWIdDto : ApplicationPartyDto
    {
        public int Id { get; set; }
        public ICollection<ApplicationPartyExtraAttachmentWIdDto> PartyExtraAttachment { get; set; }

    }
}
