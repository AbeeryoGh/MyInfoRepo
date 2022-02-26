using ChatService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.DTOs
{
  public class RoomDto :Room
  {
    public string Name { get; set; }
  }
}
