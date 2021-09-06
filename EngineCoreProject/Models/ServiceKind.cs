using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ServiceKind
    {
        public ServiceKind()
        {
            AdmService = new HashSet<AdmService>();
            QueueProcesses = new HashSet<QueueProcesses>();
        }

        public int Id { get; set; }
        public string ServiceKindNameShortcut { get; set; }
        public int EmployeeCount { get; set; }
        public int EstimatedTimePerProcess { get; set; }
        public string Symbol { get; set; }
        public short? ApplicationsPerNotary { get; set; }

        public virtual ICollection<AdmService> AdmService { get; set; }
        public virtual ICollection<QueueProcesses> QueueProcesses { get; set; }
    }
}
