using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class UserLogin : IdentityUserLogin<int>
    {
        public int Id { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public int UserId { get; set; }
        public DateTime? LoginDate { get; set; }
        public string ProviderDisplayName { get; set; }

        public virtual User User { get; set; }
    }
}
