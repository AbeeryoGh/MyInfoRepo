using EmpRepositoryLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestProjectApi
{
  public class ConnectionFactory : IDisposable
  {
    //a class hich will create a database on runtime with the name "Test_Database" 
    private bool disposedValue = false; // To detect redundant calls  

    public ApplicationDbContext CreateContextForInMemory()
    {
      var option = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "Test_Database").Options;

      var context = new ApplicationDbContext(option);
      if (context != null)
      {
        //ensures that database has been deleted from the memory with the similar name if available 
        context.Database.EnsureDeleted();
        //recreate the database
        context.Database.EnsureCreated();
      }
      //return DBContext class object for In-Memory Provider
      return context;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
        }

        disposedValue = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }

  }
}
