using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpDomainLayer.Models
{
 public class Vacation
  {
    public int Id { get; set; }
   
    public DateTime DateFrom { get; set; }
    public DateTime ToDate { get; set; }


    //public int EmpId { get; set; }
    public Employee Employee { get; set; }
   
  }
}
