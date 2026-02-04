using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ECommerce.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Publsihsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Seed Roles
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "fab4fac1-c546-41de-aebc-a14da6895711", null, "Admin", "ADMIN" },
                    { "b74ddd14-6340-4840-95c2-db12554843e5", null, "User", "USER" }
                });

            // 2. Seed Admin Users 
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "IsActive", "LockoutEnabled", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    {
                        "5f2d9e8a-b1c4-4e7a-9d6b-3f2a1c5d8e90", 0, Guid.NewGuid().ToString(),
                        "ziadaboanter@gmail.com", true, true, false, // IsActive = true
                        "ZIADABOANTER@GMAIL.COM", "ZIADABOANTER@GMAIL.COM",
                        "AQAAAAIAAYagAAAAEML2fkHPsmdTAWRtjtBWjMBkzzV/Tgl5zlxk1bVketcjmi7z4NV0vnGyNbK3+zl7hw==",
                        false, "STATIC_STAMP_ADMIN_1", false, "ziadaboanter@gmail.com"
                    },
                    {
                        "a1b2c3d4-e5f6-4a5b-bc6d-7e8f9a0b1c2d", 0, Guid.NewGuid().ToString(),
                        "za3978510@gmail.com", true, true, false, // IsActive = true
                        "ZA3978510@GMAIL.COM", "ZA3978510@GMAIL.COM",
                        "AQAAAAIAAYagAAAAEJ+OpODeeK1oOzqiV38NVxfa7sZEpvBIfPrOp+tR6iQXKKjr0u5k9uqwLb1ra9piNw==",
                        false, "STATIC_STAMP_ADMIN_2", false, "za3978510@gmail.com"
                    }
                });

            // 3. Seed User Roles (Linking Users to Admin Role)
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "fab4fac1-c546-41de-aebc-a14da6895711", "5f2d9e8a-b1c4-4e7a-9d6b-3f2a1c5d8e90" },
                    { "fab4fac1-c546-41de-aebc-a14da6895711", "a1b2c3d4-e5f6-4a5b-bc6d-7e8f9a0b1c2d" }
                });

            // 4. Seed Categories
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Devices and Gadgets", "Electronics" },
                    { 2, "Kitchen and Home tools", "Home Appliances" }
                });

            // 5. Seed DeliveryMethods
            migrationBuilder.InsertData(
                table: "DeliveryMethods",
                columns: new[] { "Id", "DeliveryTime", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "only one week", "the best", "FST", 15m },
                    { 2, "two week", "safe product", "DBL", 10m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "AspNetUserRoles", keyColumns: new[] { "RoleId", "UserId" }, keyValues: new object[] { "fab4fac1-c546-41de-aebc-a14da6895711", "5f2d9e8a-b1c4-4e7a-9d6b-3f2a1c5d8e90" });
            migrationBuilder.DeleteData(table: "AspNetUserRoles", keyColumns: new[] { "RoleId", "UserId" }, keyValues: new object[] { "fab4fac1-c546-41de-aebc-a14da6895711", "a1b2c3d4-e5f6-4a5b-bc6d-7e8f9a0b1c2d" });
            migrationBuilder.DeleteData(table: "AspNetUsers", keyColumn: "Id", keyValue: "5f2d9e8a-b1c4-4e7a-9d6b-3f2a1c5d8e90");
            migrationBuilder.DeleteData(table: "AspNetUsers", keyColumn: "Id", keyValue: "a1b2c3d4-e5f6-4a5b-bc6d-7e8f9a0b1c2d");
            migrationBuilder.DeleteData(table: "AspNetRoles", keyColumn: "Id", keyValue: "fab4fac1-c546-41de-aebc-a14da6895711");
            migrationBuilder.DeleteData(table: "AspNetRoles", keyColumn: "Id", keyValue: "b74ddd14-6340-4840-95c2-db12554843e5");
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValue: 2);
        }
    }
}