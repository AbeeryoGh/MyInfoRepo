using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class SysTranslation
    {
        public int Id { get; set; }
        public string Shortcut { get; set; }
        public string Lang { get; set; }
        public string Value { get; set; }
    }
}
