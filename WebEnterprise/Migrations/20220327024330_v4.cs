using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEnterprise.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "89f7b29d-5337-4640-bdec-17e7fccd45c5", "AQAAAAEAACcQAAAAEMXWcb5fes8Tpz9J2BqbHSQWXLTIxOLOA6ZAMgp1L5zur/rNbJ7FOeDsN6Ao+e9yhQ==", "3b94e582-162e-435a-86ba-52a86f502c97" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b66d0a4-aa39-45c7-811a-fb76c763bbe5", "AQAAAAEAACcQAAAAELc7cXwMP13p21n9t8MO6xZjJAW/+afp9523QIUSaN9ITPlQwEhgSvvOFTMgdW4I8Q==", "e9b656e8-66e3-4e54-9300-b81066818f92" });
        }
    }
}
