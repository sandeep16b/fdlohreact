using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurementFieldsToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChargeDate",
                table: "ReceivableReportHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "ReceivableReportHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "ReceivableReportHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PcardHolderFirstName",
                table: "ReceivableReportHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PcardHolderLastName",
                table: "ReceivableReportHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcurementType",
                table: "ReceivableReportHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "ReceivableReportHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 37, 49, 478, DateTimeKind.Utc).AddTicks(8693));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 37, 49, 478, DateTimeKind.Utc).AddTicks(8695));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 37, 49, 478, DateTimeKind.Utc).AddTicks(8697));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 16, 2, 37, 49, 478, DateTimeKind.Utc).AddTicks(8698));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeDate",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "PcardHolderFirstName",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "PcardHolderLastName",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "ProcurementType",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "ReceivableReportHistory");

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
        }
    }
}
