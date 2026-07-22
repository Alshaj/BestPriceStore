using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestPriceStore.Migrations
{
    /// <inheritdoc />
    public partial class ConvertUtcToYemeniTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 11, 28, 14, 657, DateTimeKind.Utc).AddTicks(2386));

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 11, 28, 14, 657, DateTimeKind.Utc).AddTicks(2392));

            // Custom SQL to convert existing UTC datetimes to Yemeni Time (+3 hours)
            migrationBuilder.Sql("UPDATE Currencies SET CreatedAt = DATEADD(hour, 3, CreatedAt);");
            migrationBuilder.Sql("UPDATE Orders SET CreatedAt = DATEADD(hour, 3, CreatedAt);");
            migrationBuilder.Sql("UPDATE Orders SET CancelledAt = DATEADD(hour, 3, CancelledAt) WHERE CancelledAt IS NOT NULL;");
            migrationBuilder.Sql("UPDATE Products SET CreatedAt = DATEADD(hour, 3, CreatedAt);");
            migrationBuilder.Sql("UPDATE Products SET UpdatedAt = DATEADD(hour, 3, UpdatedAt);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 16, 17, 41, 10, 997, DateTimeKind.Utc).AddTicks(5534));

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 16, 17, 41, 10, 997, DateTimeKind.Utc).AddTicks(5540));

            // Custom SQL to revert Yemeni Time to UTC (-3 hours)
            migrationBuilder.Sql("UPDATE Currencies SET CreatedAt = DATEADD(hour, -3, CreatedAt);");
            migrationBuilder.Sql("UPDATE Orders SET CreatedAt = DATEADD(hour, -3, CreatedAt);");
            migrationBuilder.Sql("UPDATE Orders SET CancelledAt = DATEADD(hour, -3, CancelledAt) WHERE CancelledAt IS NOT NULL;");
            migrationBuilder.Sql("UPDATE Products SET CreatedAt = DATEADD(hour, -3, CreatedAt);");
            migrationBuilder.Sql("UPDATE Products SET UpdatedAt = DATEADD(hour, -3, UpdatedAt);"); 
        }
    }
}
