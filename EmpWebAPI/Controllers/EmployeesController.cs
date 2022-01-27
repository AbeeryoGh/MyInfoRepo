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
  public class EmployeeController : ControllerBase
  {
    #region Property  
    private readonly IEmployeeService _employeeService;
    #endregion

    #region Constructor  
    public EmployeeController(IEmployeeService employeeService)
    {
      _employeeService = employeeService;
    }
    #endregion

    [HttpGet("{id}")]
    public IActionResult GetEmployee(int id)
    {
      var result = _employeeService.Get(id);
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpGet]
    public IActionResult GetAllEmployees()
    {
      var result = _employeeService.Get();
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpPost]
    public IActionResult InsertEmployee(Employee employee)
    {
      _employeeService.Post(employee);
      return Ok("Data inserted");

    }
    [HttpPut]
    public IActionResult UpdateEmployee(Employee employee)
    {
      _employeeService.Put(employee);
      return Ok("Updation done");

    }
    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(int Id)
    {
      _employeeService.Delete(Id);
      return Ok("Data Deleted");

    }
  }
}
