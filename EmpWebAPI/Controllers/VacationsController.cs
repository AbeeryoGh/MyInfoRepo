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
  public class VacationController : ControllerBase
  {
    //Create the vacation API Methods which are exposable to UI
    private readonly IVacationService _vacationService;


    #region Constructor  
    public VacationController(IVacationService vacationService)
    {
      _vacationService = vacationService;
    }
    #endregion

    //get vacation by vacationId
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
    //get all vacations
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
    //add new vacation
    [HttpPost]
    public IActionResult InsertVacation(Vacation vacation)
    {
      _vacationService.Post(vacation);
      return Ok("Data inserted");

    }
    //update vacation 
    [HttpPut]
    public IActionResult UpdateVacation(Vacation vacation)
    {
      _vacationService.Put(vacation);
      return Ok("Updation done");

    }
    //delete vacation
    [HttpDelete("{id}")]
    public IActionResult DeleteVacation(int Id)
    {
      _vacationService.Delete(Id);
      return Ok("Data Deleted");

    }
  }
}
