using ChatService.DTOs;
using ChatService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Controller
{
  [Route("api/[controller]")]
  [ApiController]
  public class MessagesController : ControllerBase
  {
    private readonly ChatDBContext _chatdbcontext;

    public MessagesController(ChatDBContext chatdbcontext)
    {
      _chatdbcontext = chatdbcontext;
    }

    [HttpPost("PostMessage")]
    public async Task<ActionResult<Room>> PostMessage(MessageDto message)
    {
      Message m = new Message();
      m.Txtmessage = message.message;
      m.Room = message.rooom;
      m.Sendfrom = message.fromuser;
      _chatdbcontext.Messages.Add(m);
      await _chatdbcontext.SaveChangesAsync();

      return Ok();
    }
  }
}
