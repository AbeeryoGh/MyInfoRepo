using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ActionRole
{
    public class PostActionRoleDto
    {
        public int ActionId { get; set; }
        public List<int> RoleId { get; set; }
    }
}
