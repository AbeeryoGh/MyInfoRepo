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
    //interface holds all methods signature which accesses by external layer for the Employee entity
    IEnumerable<Employee> Get();
    Employee Get(int id);
    void Post(Employee employee);
    void Put(Employee employee);
    void Delete(int id);
  }
}
