using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastPCB.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Title_Unique",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 5, 12, 7, 54, 795, DateTimeKind.Utc).AddTicks(9288), "100000.Tba38WWIwK53hEPbMlTckA==.YD7TDB6tiwSXaLZHp9kT52il8qibRa33aNpJ9G2sqPw=", new DateTime(2026, 4, 5, 12, 7, 54, 795, DateTimeKind.Utc).AddTicks(9289) });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title",
                table: "Projects",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Title",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 5, 11, 48, 54, 489, DateTimeKind.Utc).AddTicks(1447), "hashed_password_here", new DateTime(2026, 4, 5, 11, 48, 54, 489, DateTimeKind.Utc).AddTicks(1448) });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title_Unique",
                table: "Projects",
                column: "Title",
                unique: true);
        }
    }
}
