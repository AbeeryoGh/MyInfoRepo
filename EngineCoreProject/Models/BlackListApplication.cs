using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class BlackListApplication
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }

        public virtual Application Application { get; set; }
    }
}
