using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Credential
{
    public class PrefetchRequestDto
    {
        public string TransactionRefNo { get; set; }
        public string EmiratesID { get; set; }
        public string DocumentType { get; set; } = "POA";
    }
}
