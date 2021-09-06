using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ActionRole
    {
        public int ActionId { get; set; }
        public int RoleId { get; set; }

        public virtual AdmAction Action { get; set; }
        public virtual Role Role { get; set; }
    }
}
