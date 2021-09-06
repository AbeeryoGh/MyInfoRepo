using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class StageNamesDto
    {
        public int? Id { get; set; }
        public string stagename { get; set; }
        public int orderno { get; set; }
        public int? serviecid { get; set; }
        public string shortcut { get; set; }
        public int? stagetypeid { get; set; }
        public string stagetypename { get; set; }
        public string stagetypeshortcut { get; set;}
        public DateTime? createddate { get; set; }
        public int? periodtolate { get; set; }
        public int? periodtoarchive { get; set; }

        public DateTime? lastupdateddate { get; set; }
    }
}
