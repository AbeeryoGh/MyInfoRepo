using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class RelatedContentView
    {
        public int  Id { get; set; }
        public string TitleSortcut { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentUrl { get; set; }

    }
}
