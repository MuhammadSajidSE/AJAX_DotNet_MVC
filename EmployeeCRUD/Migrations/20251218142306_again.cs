using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeCRUD.Migrations
{
    /// <inheritdoc />
    public partial class again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_departmentId",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_departmentId",
                table: "Employees",
                column: "departmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_departmentId",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_departmentId",
                table: "Employees",
                column: "departmentId",
                unique: true);
        }
    }
}
