using EmpDomainLayer.Models;
using EmpRepositoryLayer;
using EmpServiceLayer.EmpServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    //Create the Employee API Methods which are exposable to UI
    private readonly IEmployeeService _employeeService;
    private readonly IETaskService _etaskservice;
    // private readonly IVacationService _vacationservice;


    #region Constructor  
    public EmployeeController(IEmployeeService employeeService, IETaskService etaskservice)
    {
      _employeeService = employeeService;
      _etaskservice = etaskservice;
      //_vacationservice = vacationservice;
    }
    #endregion

    //get employee by id
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
    //get all Etasks for an employee
    [HttpGet("{id}/Tasks")]
    public IActionResult GetEmployeeTasks(int id)
    {

      var result = _employeeService.Get(id);
      result.ETasks = _etaskservice.Get().Where(em => em.EmployeeId == id).ToList();
      //result.Vacations = _vacationservice.Get().Where(em => em.EmployeeId == id).ToList();

      if (!(result is null))
      {
        return Ok(result);
      }
      return BadRequest("No records found");

    }
    //get all employees
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
    //add new employee
    [HttpPost]
    public IActionResult InsertEmployee(Employee employee)
    {
      _employeeService.Post(employee);
      return Ok("Data inserted");

    }
    //update an employee
    [HttpPut]
    public IActionResult UpdateEmployee(Employee employee)
    {
      _employeeService.Put(employee);
      return Ok("Updation done");

    }
    //delete employee by its id
    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(int Id)
    {
      _employeeService.Delete(Id);
      return Ok("Data Deleted");

    }
  }
}
