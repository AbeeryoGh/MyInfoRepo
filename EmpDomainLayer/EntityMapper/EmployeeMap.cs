using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpDomainLayer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpDomainLayer.EntityMapper
{
  public class EmployeeMap : IEntityTypeConfiguration<Employee>
  {
    //define the configuration for the Employee entity that will be used when the database table will be created by the entity
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
      builder.HasKey(x => x.Id)
               .HasName("pk_Employeeid");

      builder.Property(x => x.Id).ValueGeneratedOnAdd()
                .HasColumnName("id")
                   .HasColumnType("INT");
      builder.Property(x => x.Name)
                .HasColumnName("name")
                   .HasColumnType("NVARCHAR(100)")
                   .IsRequired();
      builder.Property(x => x.Email)
              .HasColumnName("email")
                 .HasColumnType("NVARCHAR(100)")
                 .IsRequired();
      builder.Property(x => x.Position)
              .HasColumnName("position")
                 .HasColumnType("NVARCHAR(50)")
                 .IsRequired();


    }
  }
}
