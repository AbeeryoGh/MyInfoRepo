using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Log
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Description { get; set; }
        public int? Source { get; set; }
        public int? StageId { get; set; }
        public int? ApplicationId { get; set; }
        public int? ServiceId { get; set; }
        public int? NotaryId { get; set; }

        public virtual User Notary { get; set; }
        public virtual AdmService Service { get; set; }
    }
}
