using EmpDomainLayer.Models;
using EmpServiceLayer.EmpServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpWebAPI.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class ETaskController : ControllerBase
  {
    #region Property  
    private readonly IETaskService _etaskService;
    #endregion

    #region Constructor  
    public ETaskController(IETaskService etaskService)
    {
      _etaskService = etaskService;
    }
    #endregion

    [HttpGet("{id}")]
    public IActionResult GetETask(int id)
    {
      var result = _etaskService.Get(id);
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpGet]
    public IActionResult GetAllETasks()
    {
      var result = _etaskService.Get();
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpPost]
    public IActionResult InsertETask(ETask etask)
    {
      _etaskService.Post(etask);
      return Ok("Data inserted");

    }
    [HttpPut]
    public IActionResult UpdateETask(ETask etask)
    {
      _etaskService.Put(etask);
      return Ok("Updation done");

    }
    [HttpDelete("{id}")]
    public IActionResult DeleteETask(int Id)
    {
      _etaskService.Delete(Id);
      return Ok("Data Deleted");

    }
  }
}
