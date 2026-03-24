using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG005");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChargeDate",
                table: "ReceivableReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "ReceivableReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PcardHolderFirstName",
                table: "ReceivableReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PcardHolderLastName",
                table: "ReceivableReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcurementType",
                table: "ReceivableReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9420), null });

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9422), null });

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9424), null });

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2025, 9, 16, 2, 18, 2, 278, DateTimeKind.Utc).AddTicks(9426), null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ChargeDate", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ChargeDate", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ChargeDate", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ChargeDate", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType" },
                values: new object[] { null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeDate",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "PcardHolderFirstName",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "PcardHolderLastName",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "ProcurementType",
                table: "ReceivableReports");

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG001",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IT and Technology Services" });

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Financial Services and Accounting" });

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HR and Employee Services" });

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                columns: new[] { "CreatedDate", "Description" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operations and Facilities Management" });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Code", "CreatedBy", "CreatedDate", "Description", "IsActive", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[] { "ORG005", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Legal Services and Compliance", true, null, null, "Legal Department" });
        }
    }
}
