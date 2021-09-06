using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class DocumentTypeKind
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public byte? Type { get; set; }

        public virtual SysLookupValue DocumentType { get; set; }
    }
}
