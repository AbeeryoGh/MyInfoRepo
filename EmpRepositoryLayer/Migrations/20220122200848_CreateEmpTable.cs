using Microsoft.EntityFrameworkCore.Migrations;

namespace EmpRepositoryLayer.Migrations
{
    public partial class CreateEmpTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee_Task");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee_Task",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "INT", nullable: true),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Task_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Task_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Task_EmployeeId",
                table: "Employee_Task",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Task_TaskId",
                table: "Employee_Task",
                column: "TaskId");
        }
    }
}
