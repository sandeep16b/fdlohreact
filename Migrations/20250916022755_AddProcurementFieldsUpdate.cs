using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurementFieldsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "ReceivableReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "ReceivableReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 27, 55, 482, DateTimeKind.Utc).AddTicks(4465));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 27, 55, 482, DateTimeKind.Utc).AddTicks(4468));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 27, 55, 482, DateTimeKind.Utc).AddTicks(4469));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 27, 55, 482, DateTimeKind.Utc).AddTicks(4471));

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContractNumber", "PurchaseOrderNumber" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContractNumber", "PurchaseOrderNumber" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContractNumber", "PurchaseOrderNumber" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ContractNumber", "PurchaseOrderNumber" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "ReceivableReports");

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9420));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9422));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9424));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9426));
        }
    }
}
