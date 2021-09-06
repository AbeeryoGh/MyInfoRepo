using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class RelatedData
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public int? ServiceId { get; set; }
        public bool ShowApplication { get; set; }
        public bool ShowTransaction { get; set; }

        public virtual AdmService Service { get; set; }
        public virtual Template Template { get; set; }
    }
}
