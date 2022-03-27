using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEnterprise.Migrations
{
    public partial class V6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c85d5fe3-905b-4fc3-90b0-b474bfbf330a", "AQAAAAEAACcQAAAAEPToRreqmAEV62KnJ0c7PeLU3enlEUoVLvqwGxDmtMhFWe7Egrp/yOW407QQLSCxRw==", "9eb9d2c4-06c1-49ce-b9d3-2ea810fd50d3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notifications");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e7951f35-080e-4b6a-8d2a-da98b1c580f3", "AQAAAAEAACcQAAAAECEZ0ZJP5cYVtNVcSnxhy9fXHoomnlyujAXsaSWxeZ5Z2Q3/vSlbx0lYkE+cxTgSow==", "2db7ff9c-e460-478a-863e-e02eace9335f" });
        }
    }
}
