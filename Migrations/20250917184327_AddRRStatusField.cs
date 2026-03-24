using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRRStatusField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RRStatus",
                table: "ReceivableReports",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RRStatus",
                table: "ReceivableReportHistory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 18, 43, 27, 21, DateTimeKind.Utc).AddTicks(3767));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 18, 43, 27, 21, DateTimeKind.Utc).AddTicks(3769));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 18, 43, 27, 21, DateTimeKind.Utc).AddTicks(3771));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 18, 43, 27, 21, DateTimeKind.Utc).AddTicks(3772));

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1,
                column: "RRStatus",
                value: "Draft");

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                column: "RRStatus",
                value: "Draft");

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                column: "RRStatus",
                value: "Draft");

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                column: "RRStatus",
                value: "Draft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RRStatus",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "RRStatus",
                table: "ReceivableReportHistory");

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 45, 7, 549, DateTimeKind.Utc).AddTicks(3171));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 45, 7, 549, DateTimeKind.Utc).AddTicks(3176));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 45, 7, 549, DateTimeKind.Utc).AddTicks(3178));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 45, 7, 549, DateTimeKind.Utc).AddTicks(3180));
        }
    }
}
