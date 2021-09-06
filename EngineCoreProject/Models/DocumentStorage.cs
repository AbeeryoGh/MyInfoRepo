using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class DocumentStorage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int IdUser { get; set; }

        public virtual User IdUserNavigation { get; set; }
    }
}
