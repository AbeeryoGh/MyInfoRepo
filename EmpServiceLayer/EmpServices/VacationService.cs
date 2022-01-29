using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpDomainLayer.Models;
using EmpRepositoryLayer.RepositoryPattern;

namespace EmpServiceLayer.EmpServices
{
  public class VacationService : IVacationService
  {
    // a class holds all the operations for Vacation entity
    private IRepository<Vacation> _repository;

    #region Constructor  
    public VacationService(IRepository<Vacation> repository)
    {
      _repository = repository;
    }
    #endregion

    public IEnumerable<Vacation> Get()
    {
      return _repository.GetAll();
    }

    public Vacation Get(int id)
    {
      return _repository.Get(id);
    }

    public void Post(Vacation Vacation)
    {
      _repository.Insert(Vacation);
    }
    public void Put(Vacation Vacation)
    {
      _repository.Update(Vacation);
    }

    public void Delete(int id)
    {
      Vacation vacation = Get(id);
      _repository.Remove(vacation);
      _repository.SaveChanges();
    }
  }
}
