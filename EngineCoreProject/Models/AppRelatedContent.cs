using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AppRelatedContent
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public string TitleShortcut { get; set; }
        public string Content { get; set; }
        public string ContentUrl { get; set; }

        public virtual Application App { get; set; }
    }
}
