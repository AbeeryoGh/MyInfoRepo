using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpDomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpDomainLayer.EntityMapper
{
  public class VacationMap : IEntityTypeConfiguration<Vacation>
  {
    public void Configure(EntityTypeBuilder<Vacation> builder)
    {
      builder.HasKey(x => x.Id)
               .HasName("pk_Vacationid");

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

      builder.HasOne(t => t.Employee)
          .WithMany(t => t.Vacations)
          .HasForeignKey(d => d.Id)
          .HasConstraintName("FK_Task_Vacations_EmpID");
    }

  }
}
