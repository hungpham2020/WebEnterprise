using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEnterprise.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e7951f35-080e-4b6a-8d2a-da98b1c580f3", "AQAAAAEAACcQAAAAECEZ0ZJP5cYVtNVcSnxhy9fXHoomnlyujAXsaSWxeZ5Z2Q3/vSlbx0lYkE+cxTgSow==", "2db7ff9c-e460-478a-863e-e02eace9335f" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "89f7b29d-5337-4640-bdec-17e7fccd45c5", "AQAAAAEAACcQAAAAEMXWcb5fes8Tpz9J2BqbHSQWXLTIxOLOA6ZAMgp1L5zur/rNbJ7FOeDsN6Ao+e9yhQ==", "3b94e582-162e-435a-86ba-52a86f502c97" });
        }
    }
}
