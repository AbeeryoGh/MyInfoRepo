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
    #region Property  
    private IRepository<Employee> _repository;
    #endregion

    #region Constructor  
    public EmployeeService(IRepository<Employee> repository)
    {
      _repository = repository;
    }
    #endregion

    public IEnumerable<Employee> GetAllEmployees()
    {
      return _repository.GetAll();
    }

    public Employee GetEmployee(int id)
    {
      return _repository.Get(id);
    }

    public void InsertEmployee(Employee Employee)
    {
      _repository.Insert(Employee);
    }
    public void UpdateEmployee(Employee Employee)
    {
      _repository.Update(Employee);
    }

    public void DeleteEmployee(int id)
    {
      Employee Employee = GetEmployee(id);
      _repository.Remove(Employee);
      _repository.SaveChanges();
    }
  }
}
