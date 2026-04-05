using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastPCB.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Layers",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "Projects",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "MinDistance",
                table: "Projects",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 5, 12, 17, 37, 906, DateTimeKind.Utc).AddTicks(4969), new DateTime(2026, 4, 5, 12, 17, 37, 906, DateTimeKind.Utc).AddTicks(4970) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Layers",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Material",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MinDistance",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 5, 12, 7, 54, 795, DateTimeKind.Utc).AddTicks(9288), new DateTime(2026, 4, 5, 12, 7, 54, 795, DateTimeKind.Utc).AddTicks(9289) });
        }
    }
}
