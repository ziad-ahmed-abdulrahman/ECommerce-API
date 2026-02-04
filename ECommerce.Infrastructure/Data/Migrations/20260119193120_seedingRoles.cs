using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class seedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "28d77e6d-75d1-4da2-860d-67056c67d3ce", null, "Admin", "ADMIN" },
                    { "d8bbc700-75a9-4f7b-a7cd-cf4cef5c1ab7", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28d77e6d-75d1-4da2-860d-67056c67d3ce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8bbc700-75a9-4f7b-a7cd-cf4cef5c1ab7");

            migrationBuilder.RenameColumn(
                name: "OneTimeCodeExpiry",
                table: "AspNetUsers",
                newName: "OnTimeCodeExpiry");

            migrationBuilder.RenameColumn(
                name: "CodeOperation",
                table: "AspNetUsers",
                newName: "CodeOper");
        }
    }
}
