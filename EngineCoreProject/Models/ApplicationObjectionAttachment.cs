using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ApplicationObjectionAttachment
    {
        public int Id { get; set; }
        public int ObjectionId { get; set; }
        public string Attachment { get; set; }

        public virtual ApplicationObjection Objection { get; set; }
    }
}
