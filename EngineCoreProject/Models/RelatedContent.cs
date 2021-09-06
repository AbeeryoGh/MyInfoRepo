using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class RelatedContent
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public int? ServiceId { get; set; }
        public string TitleShortcut { get; set; }
        public string Content { get; set; }
        public bool IsOutput { get; set; }
        public int? StageId { get; set; }

        public virtual AdmService Service { get; set; }
        public virtual Template Template { get; set; }
    }
}
