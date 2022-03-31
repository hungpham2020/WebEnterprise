using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEnterprise.Migrations
{
    public partial class ChangeTypeOfStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "UserLikePosts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1007abe3-c19b-4b56-a292-648080b0ffbb", "AQAAAAEAACcQAAAAELsQQKAAOGZCKAqEQ/hpY29MjknekyxdYmKwRabI+YEAzQJYQPkJZrxKkku2/5ISRg==", "fe13d50f-4cdd-420a-b375-154e281f77d1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "UserLikePosts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6ec6b5f-a7de-454b-889c-ca6a424f97d1", "AQAAAAEAACcQAAAAEKRsO3TXuvEo/KSag+NEkX4eZ/1Ucnm6ApngoaUXEOrRAUACfwOlOHstHytB9Dk1nQ==", "4c316ed7-5bc3-4a9f-86e5-8a52c5d1e715" });
        }
    }
}
