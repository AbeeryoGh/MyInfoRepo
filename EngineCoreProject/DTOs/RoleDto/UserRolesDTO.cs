using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace DM.Service.ServiceModels.RoleDTO
{
    public class UserRolesDTO
    {
        public int UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}
