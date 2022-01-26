using EmpDomainLayer.Models;
using EmpRepositoryLayer.RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmpServiceLayer.EmpServices
{
  public class VacationService
  {
    #region Property  
    private IRepository<Vacation> _repository;
    #endregion

    #region Constructor  
    public VacationService(IRepository<Vacation> repository)
    {
      _repository = repository;
    }
    #endregion

    public IEnumerable<Vacation> GetAllVacations()
    {
      return _repository.GetAll();
    }

    public Vacation GetVacation(int id)
    {
      return _repository.Get(id);
    }

    public void InsertVacation(Vacation Vacation)
    {
      _repository.Insert(Vacation);
    }
    public void UpdateVacation(Vacation Vacation)
    {
      _repository.Update(Vacation);
    }

    public void DeleteVacation(int id)
    {
      Vacation Vacation = GetVacation(id);
      _repository.Remove(Vacation);
      _repository.SaveChanges();
    }
  }
}
