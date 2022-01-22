using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDomainLayer.Models
{
 public class Employee
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public string Email { get; set; }

    public List<Vacation> Vacations { get; set; }


    public List<Employee_Task> Employee_Tasks { get; set; }
  }
}
