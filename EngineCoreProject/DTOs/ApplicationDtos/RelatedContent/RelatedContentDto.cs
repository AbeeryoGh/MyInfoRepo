using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.RelatedContent
{
    public class RelatedContentDto
    {
        public int? TemplateId { get; set; }
        public int? ServiceId { get; set; }
        public int? StageId { get; set; }
        public string TitleShortcut { get; set; }
        public string Content { get; set; }
        public bool   IsOutput { get; set; }
    }
}
