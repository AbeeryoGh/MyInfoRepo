using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpDomainLayer.Models;
using EmpRepositoryLayer.RepositoryPattern;

namespace EmpServiceLayer.EmpServices
{
  public class ETaskService : IETaskService
  {
    // a class holds all the operations for ETask entity
    private IRepository<ETask> _repository;

    #region Constructor  
    public ETaskService(IRepository<ETask> repository)
    {
      _repository = repository;
    }
    #endregion

    public IEnumerable<ETask> Get()
    {
      return _repository.GetAll();
    }

    public ETask Get(int id)
    {
      return _repository.Get(id);
    }

    public void Post(ETask ETask)
    {
      _repository.Insert(ETask);
    }
    public void Put(ETask ETask)
    {
      _repository.Update(ETask);
    }

    public void Delete(int id)
    {
      ETask etashk = Get(id);
      _repository.Remove(etashk);
      _repository.SaveChanges();
    }
  }
}
