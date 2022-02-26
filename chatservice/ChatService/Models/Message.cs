using System;
using System.Collections.Generic;

#nullable disable

namespace ChatService.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public string Txtmessage { get; set; }
        public string Sendfrom { get; set; }
        public string Room { get; set; }
    }
}
