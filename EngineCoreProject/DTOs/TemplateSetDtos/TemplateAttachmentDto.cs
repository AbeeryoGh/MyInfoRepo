using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TemplateAttachmentDto
    {
        public int TemplateId   { get; set; }
        public int AttachmentId { get; set; }
        public bool? Required   { get; set; }
    }
}
