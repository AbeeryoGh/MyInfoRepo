using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.RelatedContent
{
    public class AppRelatedContentDto
    {
        public int    AppId { get; set; }
        public string TitleShortcut { get; set; }
        public string Content { get; set; }
        public string ContentUrl { get; set; }
    }
}
