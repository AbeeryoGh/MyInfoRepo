using EmpDomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmpRepositoryLayer.RepositoryPattern
{
  public interface IRepository<T> where T : BaseEntity
  {
    //a generic repository interface for the entity operations
    IEnumerable<T> GetAll();
    T Get(int Id);
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Remove(T entity);
    void SaveChanges();
  }
}
