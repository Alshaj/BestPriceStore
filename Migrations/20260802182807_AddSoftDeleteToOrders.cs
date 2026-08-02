using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestPriceStore.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 2, 21, 28, 6, 917, DateTimeKind.Unspecified).AddTicks(3177));

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 2, 21, 28, 6, 917, DateTimeKind.Unspecified).AddTicks(3177));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 18, 1, 49, 641, DateTimeKind.Unspecified).AddTicks(67));

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 18, 1, 49, 641, DateTimeKind.Unspecified).AddTicks(67));
        }
    }
}
