using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Term
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User LastUpdatedByNavigation { get; set; }
        public virtual Template Template { get; set; }
    }
}
