using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Credential
{
    public class RevokeRequest
    {
        public string vcID { get; set; }
        public string revocationReason { get; set; }
    }
}
