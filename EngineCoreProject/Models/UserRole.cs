using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class UserRole : IdentityUserRole<int>
    {
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
