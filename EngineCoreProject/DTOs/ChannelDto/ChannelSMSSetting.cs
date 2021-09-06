using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.SMSDto
{
    public class ChannelSMSSetting
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SenderID { get; set; }
        public string SourceSystemId { get; set; }
        public string HighProritySMS { get; set; }
        public string LowProritySMS { get; set; }
    }
}
