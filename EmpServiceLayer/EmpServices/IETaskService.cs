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
    //interface holds all methods signature which accesses by external layer for the Etask entity
    IEnumerable<ETask> Get();
    ETask Get(int id);
    void Post(ETask etask);
    void Put(ETask etask);
    void Delete(int id);
  }
}
