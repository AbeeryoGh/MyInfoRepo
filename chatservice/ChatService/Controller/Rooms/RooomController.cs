using ChatService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Controller.Rooms
{
  [Route("api/[controller]")]
  [ApiController]
  public class RooomController : ControllerBase
  {
    private readonly ChatDBContext _chatdbcontext;

    public RooomController(ChatDBContext chatdbcontext)
    {
      _chatdbcontext = chatdbcontext;
    }

    [HttpPost("{room}")]
    public async Task<ActionResult<Room>> PostRoom(string room)
    {
      var checkRoom = _chatdbcontext.Rooms.Where(x => x.Name == room).FirstOrDefault();
      if (checkRoom == null)
      {
        Room r = new Room();
        r.Name = room;
        _chatdbcontext.Rooms.Add(r);
        await _chatdbcontext.SaveChangesAsync();
      }
      return Ok();
    }
  }
}
