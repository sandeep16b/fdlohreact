using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class FDWIntegrationAndRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivableReports_Organizations_OrganizationCode",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "OrganizationCode",
                table: "ReceivableReportHistory");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationCode",
                table: "ReceivableReports",
                type: "nchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "FundId",
                table: "ReceivableReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OA1Id",
                table: "ReceivableReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "ReceivableReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FundId",
                table: "ReceivableReportHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OA1Id",
                table: "ReceivableReportHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "ReceivableReportHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4922));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4924));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4925));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4926));

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FundId", "OA1Id", "OrganizationCode", "OrganizationId" },
                values: new object[] { 1, 1, null, 1 });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FundId", "OA1Id", "OrganizationCode", "OrganizationId" },
                values: new object[] { 2, 2, null, 2 });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FundId", "OA1Id", "OrganizationCode", "OrganizationId" },
                values: new object[] { 3, 3, null, 3 });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FundId", "OA1Id", "OrganizationCode", "OrganizationId" },
                values: new object[] { 1, 1, null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_FundId",
                table: "ReceivableReports",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_OA1Id",
                table: "ReceivableReports",
                column: "OA1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_OrganizationId",
                table: "ReceivableReports",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivableReports_Organizations_OrganizationCode",
                table: "ReceivableReports",
                column: "OrganizationCode",
                principalTable: "Organizations",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivableReports_Organizations_OrganizationCode",
                table: "ReceivableReports");

            migrationBuilder.DropIndex(
                name: "IX_ReceivableReports_FundId",
                table: "ReceivableReports");

            migrationBuilder.DropIndex(
                name: "IX_ReceivableReports_OA1Id",
                table: "ReceivableReports");

            migrationBuilder.DropIndex(
                name: "IX_ReceivableReports_OrganizationId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "FundId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "OA1Id",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "FundId",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "OA1Id",
                table: "ReceivableReportHistory");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "ReceivableReportHistory");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationCode",
                table: "ReceivableReports",
                type: "nchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationCode",
                table: "ReceivableReportHistory",
                type: "nvarchar(10)",
                maxLength: 10,
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
                column: "OrganizationCode",
                value: "ORG001");

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                column: "OrganizationCode",
                value: "ORG002");

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                column: "OrganizationCode",
                value: "ORG003");

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                column: "OrganizationCode",
                value: "ORG001");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivableReports_Organizations_OrganizationCode",
                table: "ReceivableReports",
                column: "OrganizationCode",
                principalTable: "Organizations",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
