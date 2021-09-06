using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class TargetService
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int? TargetServiceId { get; set; }
        public int? TargetDocumentTypeId { get; set; }

        public virtual AdmService Service { get; set; }
        public virtual SysLookupValue TargetDocumentType { get; set; }
        public virtual AdmService TargetServiceNavigation { get; set; }
    }
}
