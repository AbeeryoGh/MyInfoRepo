using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class MessageInfoRes
    {
        public string Lang { get; set; }
        public int NumberOfCharacters { get; set; }
        public object ResponseMessage { get; set; }

    }


    public class MessageInfoReq
    {
        public string Lang { get; set; }
        public string MessageToSend { get; set; }

        public string PhoneNumber { get; set; }


    }
}
