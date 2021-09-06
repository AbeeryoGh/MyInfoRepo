using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class SysLookupType
    {
        public SysLookupType()
        {
            SysLookupValue = new HashSet<SysLookupValue>();
        }

        public int Id { get; set; }
        public string Value { get; set; }

        public virtual ICollection<SysLookupValue> SysLookupValue { get; set; }
    }
}
