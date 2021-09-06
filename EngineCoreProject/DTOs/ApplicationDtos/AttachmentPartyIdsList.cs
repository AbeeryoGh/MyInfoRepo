using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class AttachmentPartyIdsList
    {
        public List<int> Parties { get; set; }
        public List<int> Attachments { get; set; }
        public List<int> ExtraAttachments { get; set; }
    }
}
