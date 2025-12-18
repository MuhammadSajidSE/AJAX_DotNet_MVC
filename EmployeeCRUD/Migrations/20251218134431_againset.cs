using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeCRUD.Migrations
{
    /// <inheritdoc />
    public partial class againset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_departmentId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_departmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "department",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "departmentId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "departmentId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_departmentId",
                table: "Employees",
                column: "departmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_departmentId",
                table: "Employees",
                column: "departmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_departmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_departmentId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "departmentId",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "departmentId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_departmentId",
                table: "AspNetUsers",
                column: "departmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_departmentId",
                table: "AspNetUsers",
                column: "departmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
