using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ESignPayload
    {
        public int appPartyId{ get; set; }
        public int userId { get; set; }
        public int appId { get; set; }
        public string Base64Sign { get; set; }
        public int signType { get; set; }
    }
}
