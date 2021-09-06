using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos.ModelView
{
    public class TemplateView
    {
        // public int? DocumentTypeId { get; set; }
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public string Title { get; set; }
        public string DocumentType { get; set; }
        public string TitleShortcut { get; set; }
        public byte? Type { get; set; }
        public string DocumentTypeLoukup { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool ShowApplication { get; set; }
        public bool ShowTransaction { get; set; }
    }
}
