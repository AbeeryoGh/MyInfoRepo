using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmpDomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpDomainLayer.EntityMapper
{
  public class TaskMap : IEntityTypeConfiguration<ETask>
  {
    public void Configure(EntityTypeBuilder<ETask> builder)
    {
      builder.HasKey(x => x.Id)
               .HasName("pk_taskid");

      builder.Property(x => x.Id).ValueGeneratedOnAdd()
                .HasColumnName("id")
                   .HasColumnType("INT");

      //builder.Property(x => x.EmpId)
      //          .HasColumnName("userid")
      //             .HasColumnType("INT")
      //             .IsRequired();
      builder.Property(x => x.DateFrom)
              .HasColumnName("datefrom")
                 .HasColumnType("datetime")
                 .IsRequired();
      builder.Property(x => x.ToDate)
              .HasColumnName("todate")
                 .HasColumnType("datetime")
                 .IsRequired();

      builder.Property(x => x.Priority)
            .HasColumnName("priority")
               .HasColumnType("bit");

      builder.HasOne(t => t.Employee)
           .WithMany(t => t.ETasks)
           .HasForeignKey(d => d.Id)
           .HasConstraintName("FK_Task_Employee_EmpID");


    }

  }
}
