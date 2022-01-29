using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpDomainLayer.Models;
using EmpRepositoryLayer.RepositoryPattern;

namespace EmpServiceLayer.EmpServices
{
  public class EmployeeService : IEmployeeService
  {
    // a class holds all the operations for Employee entity
    private IRepository<Employee> _repository;

    #region Constructor  
    public EmployeeService(IRepository<Employee> repository)
    {
      _repository = repository;
    }
    #endregion

    public IEnumerable<Employee> Get()
    {
      return _repository.GetAll();
    }

    public Employee Get(int id)
    {
      return _repository.Get(id);
    }

    public void Post(Employee Employee)
    {
      _repository.Insert(Employee);
    }
    public void Put(Employee Employee)
    {
      _repository.Update(Employee);
    }

    public void Delete(int id)
    {
      Employee Employee = Get(id);
      _repository.Remove(Employee);
      _repository.SaveChanges();
    }
  }
}
