using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class RoleClaim : IdentityRoleClaim<int>
    {
       public virtual Role Role { get; set; }
    }
}
