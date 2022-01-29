using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDomainLayer.Models
{
  //ETask class which represent ETask Table
  public class ETask : BaseEntity
  {
    //public int Id { get; set; }
    //public int UserId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime ToDate { get; set; }
    public bool Priority { get; set; }


    // public int EmpId { get; set; }
    //public Employee Employee { get; set; }

    [Key, ForeignKey("Employee")]
    public int EmployeeId { get; set; }

    // public List<Employee_Task> Employee_Tasks { get; set; }

  }
}
