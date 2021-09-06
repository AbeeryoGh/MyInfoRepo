using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class DocumentType
    {
        public int Id { get; set; }
        public string Shortcut { get; set; }
        public byte? Type { get; set; }
    }
}
