using Microsoft.EntityFrameworkCore.Migrations;

namespace EmpRepositoryLayer.Migrations
{
    public partial class CreateEmployeeDBTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "Vacation");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "Task");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpId",
                table: "Vacation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmpId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
