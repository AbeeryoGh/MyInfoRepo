using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmpDomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpDomainLayer.EntityMapper
{
  class TaskMap : IEntityTypeConfiguration<Task>
  {
    public void Configure(EntityTypeBuilder<Task> builder)
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

    }

  }
}
