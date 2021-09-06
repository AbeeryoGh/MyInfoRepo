using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Role : IdentityRole<int>
    {
        public Role() : base()
        {
            RoleClaim = new HashSet<RoleClaim>();
            UserRole = new HashSet<UserRole>();
        }

        public Role(string roleName) : base(roleName)
        {

        }

        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string RecStatus { get; set; }

        public virtual ICollection<RoleClaim> RoleClaim { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
