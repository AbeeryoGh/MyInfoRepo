using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.RoleDto
{
    public class RoleTabGetDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int TabOrder { get; set; }
        public string Name { get; set; }
        public bool HasAccess { get; set; }
    }
}
