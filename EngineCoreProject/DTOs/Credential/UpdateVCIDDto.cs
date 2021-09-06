using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Credential
{
    public class UpdateVCIDDto
    {
        public string transactionRefNo { get;set;}
        public string transactionRefOfRequest { get; set; }
        public string VCID { get; set; }
    }
}
