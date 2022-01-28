using EmpDomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XUnitTestProjectApi
{
  public class InMemoryDataProviderTest
  {
    [Fact]
    public void Task_Employee_Test()
    {
      //Arrange    
      var factory = new ConnectionFactory();

      //Get the instance of ApplicationDBContext  
      var context = factory.CreateContextForInMemory();

      var employee = new Employee() { Name = "Test Name 3", Position = "Test Position 3", Email = "Test Email 3" };
      var employee2 = new Employee() { Name = "Test Name 3", Position = "Test Position 3", Email = "Test Email 3" };

      //Act    
      var data = context.Employee.Add(employee);
      var data2 = context.Employee.Add(employee2);
      context.SaveChanges();

      //Assert    
      //Get the Employees count  
      var employeeCount = context.Employee.Count();
      if (employeeCount != 0)
      {
        Assert.Equal(2, employeeCount);
      }

    }
  }
}
