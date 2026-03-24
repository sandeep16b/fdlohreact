using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProcurementToSeparateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeDate",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
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

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "ReceivableReports");

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

            migrationBuilder.AddColumn<int>(
                name: "ProcurementMethodId",
                table: "ReceivableReports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcurementMethodId",
                table: "ReceivableReportHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcurementTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcurementTypeCode = table.Column<string>(type: "nchar(10)", maxLength: 10, nullable: false),
                    ChargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PcardHolderFirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PcardHolderLastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GroupId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementMethods_ProcurementTypes_ProcurementTypeCode",
                        column: x => x.ProcurementTypeCode,
                        principalTable: "ProcurementTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "ProcurementTypes",
                columns: new[] { "Code", "CreatedBy", "CreatedDate", "Description", "IsActive", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { "CONTRACT", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Purchase under existing contract", true, null, null, "Contract Purchase" },
                    { "DIRECT", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Direct purchase without formal process", true, null, null, "Direct Purchase" },
                    { "PCARD", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Purchase using procurement card", true, null, null, "P-Card Purchase" },
                    { "PO", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Purchase using purchase order", true, null, null, "Purchase Order" }
                });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProcurementMethodId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProcurementMethodId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProcurementMethodId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProcurementMethodId",
                value: 1);

            migrationBuilder.InsertData(
                table: "ProcurementMethods",
                columns: new[] { "Id", "ChargeDate", "ContractNumber", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "GroupId", "IsDeleted", "ModifiedBy", "ModifiedDate", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementTypeCode", "PurchaseOrderNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "GRP001", false, null, null, "John", "Smith", "PCARD", null },
                    { 2, null, null, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, null, null, null, null, "PO", "PO-2024-001" },
                    { 3, null, "CNT-2024-001", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, null, null, null, null, "CONTRACT", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_ProcurementMethodId",
                table: "ReceivableReports",
                column: "ProcurementMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementMethods_ChargeDate",
                table: "ProcurementMethods",
                column: "ChargeDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementMethods_ContractNumber",
                table: "ProcurementMethods",
                column: "ContractNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementMethods_CreatedDate",
                table: "ProcurementMethods",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementMethods_IsDeleted",
                table: "ProcurementMethods",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementMethods_ProcurementTypeCode",
                table: "ProcurementMethods",
                column: "ProcurementTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementMethods_PurchaseOrderNumber",
                table: "ProcurementMethods",
                column: "PurchaseOrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTypes_CreatedDate",
                table: "ProcurementTypes",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTypes_IsActive",
                table: "ProcurementTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTypes_Name",
                table: "ProcurementTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivableReports_ProcurementMethods_ProcurementMethodId",
                table: "ReceivableReports",
                column: "ProcurementMethodId",
                principalTable: "ProcurementMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceivableReports_ProcurementMethods_ProcurementMethodId",
                table: "ReceivableReports");

            migrationBuilder.DropTable(
                name: "ProcurementMethods");

            migrationBuilder.DropTable(
                name: "ProcurementTypes");

            migrationBuilder.DropIndex(
                name: "IX_ReceivableReports_ProcurementMethodId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "ProcurementMethodId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "ProcurementMethodId",
                table: "ReceivableReportHistory");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChargeDate",
                table: "ReceivableReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "ReceivableReports",
                type: "nvarchar(100)",
                maxLength: 100,
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

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "ReceivableReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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
                value: new DateTime(2025, 9, 17, 17, 26, 43, 304, DateTimeKind.Utc).AddTicks(2578));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG002",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 26, 43, 304, DateTimeKind.Utc).AddTicks(2581));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG003",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 26, 43, 304, DateTimeKind.Utc).AddTicks(2583));

            migrationBuilder.UpdateData(
                table: "Organizations",
                keyColumn: "Code",
                keyValue: "ORG004",
                column: "CreatedDate",
                value: new DateTime(2025, 9, 17, 17, 26, 43, 304, DateTimeKind.Utc).AddTicks(2585));

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ChargeDate", "ContractNumber", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType", "PurchaseOrderNumber" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ChargeDate", "ContractNumber", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType", "PurchaseOrderNumber" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ChargeDate", "ContractNumber", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType", "PurchaseOrderNumber" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ChargeDate", "ContractNumber", "GroupId", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementType", "PurchaseOrderNumber" },
                values: new object[] { null, null, null, null, null, null, null });
        }
    }
}
