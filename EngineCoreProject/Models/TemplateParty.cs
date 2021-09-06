using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class TemplateParty
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int PartyId { get; set; }
        public bool? Required { get; set; }
        public bool? SignRequired { get; set; }

        public virtual SysLookupValue Party { get; set; }
        public virtual Template Template { get; set; }
    }
}
