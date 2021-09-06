using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Ocrdocuments
    {
        public Ocrdocuments()
        {
            OcrdocumentFields = new HashSet<OcrdocumentFields>();
        }

        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public double? Score { get; set; }
        public int? Xmin { get; set; }
        public int? Xmax { get; set; }
        public int? Ymin { get; set; }
        public int? Ymax { get; set; }
        public string BboxImg { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<OcrdocumentFields> OcrdocumentFields { get; set; }
    }
}
