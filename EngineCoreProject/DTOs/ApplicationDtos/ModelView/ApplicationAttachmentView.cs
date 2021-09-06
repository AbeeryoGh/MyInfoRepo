using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class ApplicationAttachmentView
    {
        public int     Id             { get; set; }
        public int     AttachmentId   { get; set; }
        public int     TranslationId  { get; set; }
        public string  AttachmentName { get; set; }
        public string  FileName       { get; set; }
        public string  Note           { get; set; }

        // public string RequiredText { get; set; }
        // public bool Required { get; set; }
    }
}
