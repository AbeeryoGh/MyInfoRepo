using System;
using System.Collections.Generic;
using EngineCoreProject.Models;

namespace EngineCoreProject.DTOs.RoleDto
{
    public class RoleWithPermissionsPostDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> RoleNameShortCut { get; set; }
        public Dictionary <string, List<string>> Permissions { get; set; }

    }
}
