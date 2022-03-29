using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEnterprise.Migrations
{
    public partial class NewDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6ec6b5f-a7de-454b-889c-ca6a424f97d1", "AQAAAAEAACcQAAAAEKRsO3TXuvEo/KSag+NEkX4eZ/1Ucnm6ApngoaUXEOrRAUACfwOlOHstHytB9Dk1nQ==", "4c316ed7-5bc3-4a9f-86e5-8a52c5d1e715" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b66d0a4-aa39-45c7-811a-fb76c763bbe5", "AQAAAAEAACcQAAAAELc7cXwMP13p21n9t8MO6xZjJAW/+afp9523QIUSaN9ITPlQwEhgSvvOFTMgdW4I8Q==", "e9b656e8-66e3-4e54-9300-b81066818f92" });
        }
    }
}
