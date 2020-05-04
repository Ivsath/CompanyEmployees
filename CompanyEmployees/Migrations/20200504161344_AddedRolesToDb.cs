using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyEmployees.Migrations
{
    public partial class AddedRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c422007e-df0e-4bcc-bde6-103223eae64d", "ef617cd6-e4a1-4605-8679-ad7266c6580a", "Manager", "MANAGER" },
                    { "82c38260-5655-442f-bad7-e6bdfb56634b", "1925ce83-33de-4ee3-b5a1-506b36332b6f", "Administrator", "ADMINISTRATOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "82c38260-5655-442f-bad7-e6bdfb56634b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c422007e-df0e-4bcc-bde6-103223eae64d");
        }
    }
}
