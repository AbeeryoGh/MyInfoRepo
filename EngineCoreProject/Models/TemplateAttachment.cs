using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class TemplateAttachment
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int AttachmentId { get; set; }
        public bool? Required { get; set; }

        public virtual SysLookupValue Attachment { get; set; }
        public virtual Template Template { get; set; }
    }
}
