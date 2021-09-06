using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class TargetApplication
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public int? TargetAppId { get; set; }
        public string TargetAppDesc { get; set; }
        public string TargetAppDocument { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Application App { get; set; }
        public virtual Application TargetApp { get; set; }
    }
}
