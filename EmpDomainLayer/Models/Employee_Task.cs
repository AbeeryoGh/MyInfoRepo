using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDomainLayer.Models
{
  public class Employee_Task
  {
    public int Id { get; set; }


    public int EmpId { get; set; }
    public Employee Employee { get; set; }


    public int TaskId { get; set; }
    public Task Task { get; set; }

  }
}
