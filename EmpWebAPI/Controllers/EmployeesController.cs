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
  [Route("[controller]")]
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

    [HttpGet]
    public IActionResult GetEmployee(int id)
    {
      var result = _employeeService.GetEmployee(id);
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpGet(nameof(GetAllEmployees))]
    public IActionResult GetAllEmployees()
    {
      var result = _employeeService.GetAllEmployees();
      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    [HttpPost(nameof(InsertEmployee))]
    public IActionResult InsertEmployee(Employee employee)
    {
      _employeeService.InsertEmployee(employee);
      return Ok("Data inserted");

    }
    [HttpPut(nameof(UpdateEmployee))]
    public IActionResult UpdateEmployee(Employee employee)
    {
      _employeeService.UpdateEmployee(employee);
      return Ok("Updation done");

    }
    [HttpDelete(nameof(DeleteEmployee))]
    public IActionResult DeleteEmployee(int Id)
    {
      _employeeService.DeleteEmployee(Id);
      return Ok("Data Deleted");

    }
  }
}
