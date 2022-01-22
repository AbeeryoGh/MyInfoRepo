using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDomainLayer.Models
{
 public class Task
  {
    public int Id { get; set; }
    //public int UserId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime ToDate { get; set; }
    public bool Priority { get; set; }


    public List<Employee_Task> Employee_Tasks { get; set; }

  }
}
