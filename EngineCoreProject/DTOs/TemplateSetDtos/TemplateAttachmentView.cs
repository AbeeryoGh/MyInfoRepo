using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TemplateAttachmentView
    {
        public int Id { get; set; }

        public string AttachmentName  { get; set; }
        public bool? Required { get; set; }
  
    }
}
