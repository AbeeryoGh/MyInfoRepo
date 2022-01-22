using Microsoft.EntityFrameworkCore;
using System;
using EmpDomainLayer.EntityMapper;

namespace EmpRepositoryLayer
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new EmployeeMap());

      base.OnModelCreating(modelBuilder);
    }

  }
}
