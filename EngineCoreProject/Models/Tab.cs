using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Tab
    {
        public Tab()
        {
            InverseParent = new HashSet<Tab>();
        }

        public int Id { get; set; }
        public string TabNameShortcut { get; set; }
        public string Link { get; set; }
        public int? ParentId { get; set; }
        public int TabOrder { get; set; }
        public byte[] Icon { get; set; }
        public string IconString { get; set; }

        public virtual Tab Parent { get; set; }
        public virtual ICollection<Tab> InverseParent { get; set; }
    }
}
