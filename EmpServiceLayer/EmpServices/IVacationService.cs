using EmpDomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpServiceLayer.EmpServices
{
  public interface IVacationService
  {
    IEnumerable<Vacation> Get();
    Vacation Get(int id);
    void Post(Vacation etask);
    void Put(Vacation etask);
    void Delete(int id);
  }
}
