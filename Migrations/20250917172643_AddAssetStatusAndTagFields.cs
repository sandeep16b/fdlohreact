using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetStatusAndTagFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetStatus",
                table: "Assets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagAttestedBy",
                table: "Assets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TagAttestedDate",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagPrintedBy",
                table: "Assets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TagPrintedDate",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueTagNumber",
                table: "Assets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetStatus",
                table: "AssetHistory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagAttestedBy",
                table: "AssetHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TagAttestedDate",
                table: "AssetHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagPrintedBy",
                table: "AssetHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TagPrintedDate",
                table: "AssetHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueTagNumber",
                table: "AssetHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AssetStatus", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[] { "Open", null, null, null, null, null });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetStatus",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TagAttestedBy",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TagAttestedDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TagPrintedBy",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TagPrintedDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UniqueTagNumber",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetStatus",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "TagAttestedBy",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "TagAttestedDate",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "TagPrintedBy",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "TagPrintedDate",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "UniqueTagNumber",
                table: "AssetHistory");

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
    }
}
