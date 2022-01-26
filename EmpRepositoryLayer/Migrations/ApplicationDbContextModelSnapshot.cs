﻿// <auto-generated />
using System;
using EmpRepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmpRepositoryLayer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EmpDomainLayer.Models.ETask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("INT")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateFrom")
                        .HasColumnName("datefrom")
                        .HasColumnType("datetime");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("INT");

                    b.Property<bool>("Priority")
                        .HasColumnName("priority")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ToDate")
                        .HasColumnName("todate")
                        .HasColumnType("datetime");

                    b.HasKey("Id")
                        .HasName("pk_taskid");

                    b.HasIndex("EmployeeId");

                    b.ToTable("ETask");
                });

            modelBuilder.Entity("EmpDomainLayer.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("INT")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("NVARCHAR(100)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnName("position")
                        .HasColumnType("NVARCHAR(50)");

                    b.HasKey("Id")
                        .HasName("pk_Employeeid");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("EmpDomainLayer.Models.Vacation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("INT")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateFrom")
                        .HasColumnName("datefrom")
                        .HasColumnType("datetime");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("INT");

                    b.Property<DateTime>("ToDate")
                        .HasColumnName("todate")
                        .HasColumnType("datetime");

                    b.HasKey("Id")
                        .HasName("pk_Vacationid");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Vacation");
                });

            modelBuilder.Entity("EmpDomainLayer.Models.ETask", b =>
                {
                    b.HasOne("EmpDomainLayer.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");
                });

            modelBuilder.Entity("EmpDomainLayer.Models.Vacation", b =>
                {
                    b.HasOne("EmpDomainLayer.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");
                });
#pragma warning restore 612, 618
        }
    }
}
