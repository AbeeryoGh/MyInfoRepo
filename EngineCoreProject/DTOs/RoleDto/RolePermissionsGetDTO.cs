using EngineCoreProject.DTOs.TabDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCoreProject.DTOs.RoleDto
{
    public class RolePermissionsGetDTO
    {
        public int RoleId { get; set; }
        public dynamic ActionPermissions { get; set; }
        public List<UserTabGetDTO> TabPermissions { get; set; }

    }
}
