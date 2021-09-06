using System;
using System.Collections.Generic;
using EngineCoreProject.Models;

namespace EngineCoreProject.DTOs.RoleDto
{
    public class UsersRolesDto
    {
        public int userid { get; set; }
        public string userName { get; set; }
        public string Email { get; set; }
        public List<Roles> roles { get; set; }


    }
}


public class Roles
{
    public int id { get; set; }
    public string RoleName { get; set; }

}