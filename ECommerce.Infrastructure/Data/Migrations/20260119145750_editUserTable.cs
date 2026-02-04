using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class editUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationCodeExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ActivationCode",
                table: "AspNetUsers");

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodeOperation",
                table: "AspNetUsers",
                newName: "ActivationCode");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationCodeExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}
