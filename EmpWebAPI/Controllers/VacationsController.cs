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
  [Route("api/[controller]")]
  [ApiController]
  public class VacationController : ControllerBase
  {
    #region Property  
    private readonly IVacationService _vacationService;
    #endregion

    #region Constructor  
    public VacationController(IVacationService vacationService)
    {
      _vacationService = vacationService;
    }
    #endregion

    [HttpGet("{id}")]
    public IActionResult GetVacation(int id)
    {
      var result = _vacationService.Get(id);
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpGet]
    public IActionResult GetAllVacations()
    {
      var result = _vacationService.Get();
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpPost]
    public IActionResult InsertVacation(Vacation vacation)
    {
      _vacationService.Post(vacation);
      return Ok("Data inserted");

    }
    [HttpPut]
    public IActionResult UpdateVacation(Vacation vacation)
    {
      _vacationService.Put(vacation);
      return Ok("Updation done");

    }
    [HttpDelete("{id}")]
    public IActionResult DeleteVacation(int Id)
    {
      _vacationService.Delete(Id);
      return Ok("Data Deleted");

    }
  }
}
