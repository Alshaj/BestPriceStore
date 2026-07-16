using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestPriceStore.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiCurrencyToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalAmountYer");

            migrationBuilder.AddColumn<double>(
                name: "TotalAmountSar",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "OrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 16, 17, 21, 54, 685, DateTimeKind.Utc).AddTicks(9919));

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 16, 17, 21, 54, 685, DateTimeKind.Utc).AddTicks(9923));

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_CurrencyId",
                table: "OrderProducts",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Currencies_CurrencyId",
                table: "OrderProducts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Currencies_CurrencyId",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_CurrencyId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "TotalAmountSar",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "OrderProducts");

            migrationBuilder.RenameColumn(
                name: "TotalAmountYer",
                table: "Orders",
                newName: "TotalAmount");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 13, 19, 47, 8, 405, DateTimeKind.Utc).AddTicks(1771));

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 13, 19, 47, 8, 405, DateTimeKind.Utc).AddTicks(1778));
        }
    }
}
