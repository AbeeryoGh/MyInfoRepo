using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos.ModelView
{
    public class AttachmentView
    {
        public int AttachmentId { get; set; }
        public int RelationId { get; set; }
        public int TranslationId { get; set; }
        public string AttachmentName { get; set; }
        public string RequiredText { get; set; }
        public bool Required { get; set; }
    }
}
