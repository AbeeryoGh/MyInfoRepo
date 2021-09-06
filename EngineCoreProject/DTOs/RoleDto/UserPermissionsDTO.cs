using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCoreProject.DTOs.RoleDto
{
    public class UserPermissionsDTO
    {
        public int UserID { get; set; }

        public List<RoleClaim> Permissions { get; set; }

        public UserPermissionsDTO()
        {
            Permissions = new List<RoleClaim>();
        }

    }
}
