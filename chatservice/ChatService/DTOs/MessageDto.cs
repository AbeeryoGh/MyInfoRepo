using ChatService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.DTOs
{
  public class MessageDto : Message
  {
    public string message { get; set; }
    public string fromuser { get; set; }
    public string rooom { get; set; }
  }
}
