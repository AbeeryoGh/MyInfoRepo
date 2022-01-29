using Microsoft.EntityFrameworkCore;
using System;
using EmpDomainLayer.EntityMapper;
using EmpDomainLayer.Models;

namespace EmpRepositoryLayer
{
  // a data access context class
  public class ApplicationDbContext : DbContext
  {

    public ApplicationDbContext()
    {
    }
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Employee> Employee { get; set; }
    public virtual DbSet<ETask> ETask { get; set; }
    public virtual DbSet<Vacation> Vacation { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseSqlServer("Name=EmpDBCon");
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new EmployeeMap());
      modelBuilder.ApplyConfiguration(new ETaskMap());
      modelBuilder.ApplyConfiguration(new VacationMap());

      //  modelBuilder.Entity<Employee>()
      //   .HasMany(c => c.Tasks)
      //   .WithOne(e => e.Employee);

      //  modelBuilder.Entity<Employee>()
      //.HasMany(c => c.Vacations)
      //.WithOne(e => e.Employee);


    }

  }
}
