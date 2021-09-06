using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AdmServiceDocumentType
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DocumentTypeId { get; set; }

        public virtual SysLookupValue DocumentType { get; set; }
        public virtual AdmService Service { get; set; }
    }
}
