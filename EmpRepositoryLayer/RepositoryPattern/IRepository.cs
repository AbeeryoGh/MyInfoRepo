using EmpDomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmpRepositoryLayer.RepositoryPattern
{
  public interface IRepository<T> where T : BaseEntity
  {
    IEnumerable<T> GetAll();
    T Get(int Id);
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Remove(T entity);
    void SaveChanges();
  }
}
