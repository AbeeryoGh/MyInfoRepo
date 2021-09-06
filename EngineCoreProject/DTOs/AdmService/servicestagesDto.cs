using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class servicestagesDto
    {
        public int? serviceid { get; set; }
        public int? stageid { get; set; }
        public string stagename { get; set; }
        public string stageshortcut { get; set; }
    }
}
