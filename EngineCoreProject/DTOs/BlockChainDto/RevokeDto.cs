using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.BlockChainDto
{
    public class RevokeDto
    {
        public string VcID { get; set; }
        public string RevocationReason { get; set; }
    }
}
