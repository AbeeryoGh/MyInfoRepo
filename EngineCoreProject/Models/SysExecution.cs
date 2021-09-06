using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class SysExecution
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int ExecutionOrder { get; set; }
        public string ToExecute { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Method { get; set; }

        public virtual AdmAction Action { get; set; }
    }
}
