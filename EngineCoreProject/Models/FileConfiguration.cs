using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class FileConfiguration
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public int? MaxSize { get; set; }
        public int? MinSize { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
    }
}
