using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Administrators",
                columns: new[] { "Id", "Email", "Profile", "Password" },
                values: new object[] { "1", "Administrator@teste.com", "Adm", "123" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: "1");
        }
    }
}
