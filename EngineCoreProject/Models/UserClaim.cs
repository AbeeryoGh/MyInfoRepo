using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class UserClaim : IdentityUserClaim<int>
    {
        public virtual User User { get; set; }
    }
}
