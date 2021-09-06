using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TemplateDto
    {
        public int? DocumentTypeId { get; set; }
        public string TitleShortcut { get; set; }
        public byte? Type { get; set; }
    }
}
