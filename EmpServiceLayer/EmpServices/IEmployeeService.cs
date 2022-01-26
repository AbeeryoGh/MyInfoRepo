using EmpDomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpServiceLayer.EmpServices
{
  public interface IEmployeeService
  {
    IEnumerable<Employee> GetAllEmployees();
    Employee GetEmployee(int id);
    void InsertEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(int id);
  }
}
