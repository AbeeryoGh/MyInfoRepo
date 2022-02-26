using ChatService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Controller
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly ChatDBContext _chatdbcontext;

    public UsersController(ChatDBContext chatdbcontext)
    {
      _chatdbcontext = chatdbcontext;
    }

    [HttpGet("GetById")]
    public async Task<ActionResult<User>> GetByUserNameandPass([FromBody] User user)
    {

      var result = await _chatdbcontext.Users.Where(x => x.Username == user.Username && x.Pass == user.Pass).FirstOrDefaultAsync();

      return Ok(result);
   
    }

    [HttpGet("GetUsers")]
    public async Task<ActionResult<User>> GetUsers()
    {

      var result = await _chatdbcontext.Users.ToListAsync();

      return Ok(result);

    }

   


    //[HttpGet("GetByUserNameandPass/{username}/{password}")]
    //public async Task<IActionResult> GetByUserNameandPass(string username, string password)
    //{
    //  if (username == null || password == null)
    //    return BadRequest("username or password are empty");
    //  var result = await _user.GetByUserNameandPass(username, password);

    //  return Ok(result);

    //}

  }
}
