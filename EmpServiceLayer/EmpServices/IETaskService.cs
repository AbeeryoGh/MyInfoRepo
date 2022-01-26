using EmpDomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpServiceLayer.EmpServices
{
  public interface IETaskService
  {
    IEnumerable<ETask> Get();
    ETask Get(int id);
    void Post(ETask etask);
    void Put(ETask etask);
    void Delete(int id);
  }
}
