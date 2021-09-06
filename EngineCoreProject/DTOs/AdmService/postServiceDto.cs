using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class postServiceDto
    {
        public int? UgId { get; set; }
        public string NameShortcut { get; set; }
        public int? TargetService { get; set; }
        public DateTime? createddate { get; set; }
        public DateTime? updateddate { get; set; }

      
    }
}
