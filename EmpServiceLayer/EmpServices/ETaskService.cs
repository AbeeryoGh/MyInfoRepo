using EmpDomainLayer.Models;
using EmpRepositoryLayer.RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmpServiceLayer.EmpServices
{
  public class TaskService
  {
    #region Property  
    private IRepository<ETask> _repository;
    #endregion

    #region Constructor  
    public TaskService(IRepository<ETask> repository)
    {
      _repository = repository;
    }
    #endregion

    public IEnumerable<ETask> GetAllTasks()
    {
      return _repository.GetAll();
    }

    public ETask GetTask(int id)
    {
      return _repository.Get(id);
    }

    public void InsertTask(ETask Task)
    {
      _repository.Insert(Task);
    }
    public void UpdateTask(ETask Task)
    {
      _repository.Update(Task);
    }

    public void DeleteTask(int id)
    {
      ETask Task = GetTask(id);
      _repository.Remove(Task);
      _repository.SaveChanges();
    }
  }
}
