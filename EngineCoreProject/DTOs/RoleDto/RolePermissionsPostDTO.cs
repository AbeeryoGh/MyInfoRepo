using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCoreProject.DTOs.RoleDto
{
    public class RolePermissionsPostDTO
    {
        public int RoleID { get; set; }

        public List<Int32> ActionPermissions { get; set; }
        public List<Int32> TabPermissions { get; set; }

    }
}
