using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class OcrdocumentFields
    {
        public int Id { get; set; }
        public string FieldClass { get; set; }
        public int? DocumentId { get; set; }
        public double? Score { get; set; }
        public int? Xmin { get; set; }
        public int? Xmax { get; set; }
        public int? Ymin { get; set; }
        public int? Ymax { get; set; }
        public string Text { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Ocrdocuments Document { get; set; }
    }
}
