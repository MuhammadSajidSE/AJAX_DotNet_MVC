using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeCRUD.Migrations
{
    /// <inheritdoc />
    public partial class againsetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_departmentId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_departmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "departmentId",
                table: "AspNetUsers");
        }
    }
}
