using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReceivableReportId = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ObjectCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Room = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsOwnedByCounty = table.Column<bool>(type: "bit", nullable: false),
                    CountyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HistoryUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalCreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetHistory", x => x.HistoryId);
                    table.CheckConstraint("CK_AssetHistory_OperationType", "[OperationType] IN ('INSERT', 'UPDATE', 'DELETE')");
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    County = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ObjectCodes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectCodes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ReceivableReportHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceivableReportId = table.Column<int>(type: "int", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrganizationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    County = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttestedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HistoryUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalCreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivableReportHistory", x => x.HistoryId);
                    table.CheckConstraint("CK_ReceivableReportHistory_OperationType", "[OperationType] IN ('INSERT', 'UPDATE', 'DELETE')");
                });

            migrationBuilder.CreateTable(
                name: "ReceivableReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationCode = table.Column<string>(type: "nchar(10)", maxLength: 10, nullable: false),
                    LocationCode = table.Column<string>(type: "nchar(10)", maxLength: 10, nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    County = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttestedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivableReports", x => x.Id);
                    table.CheckConstraint("CK_ReceivableReport_OrderStatus", "[OrderStatus] IN ('Partial', 'Complete')");
                    table.ForeignKey(
                        name: "FK_ReceivableReports_Locations_LocationCode",
                        column: x => x.LocationCode,
                        principalTable: "Locations",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReceivableReports_Organizations_OrganizationCode",
                        column: x => x.OrganizationCode,
                        principalTable: "Organizations",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceivableReportId = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ObjectCode = table.Column<string>(type: "nchar(10)", maxLength: 10, nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Room = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsOwnedByCounty = table.Column<bool>(type: "bit", nullable: false),
                    CountyCode = table.Column<string>(type: "nchar(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Counties_CountyCode",
                        column: x => x.CountyCode,
                        principalTable: "Counties",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Assets_ObjectCodes_ObjectCode",
                        column: x => x.ObjectCode,
                        principalTable: "ObjectCodes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Assets_ReceivableReports_ReceivableReportId",
                        column: x => x.ReceivableReportId,
                        principalTable: "ReceivableReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Counties",
                columns: new[] { "Code", "CreatedBy", "CreatedDate", "IsActive", "ModifiedBy", "ModifiedDate", "Name", "State" },
                values: new object[,]
                {
                    { "CTY001", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "New York County", "NY" },
                    { "CTY002", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "Los Angeles County", "CA" },
                    { "CTY003", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "Cook County", "IL" },
                    { "CTY004", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "Harris County", "TX" },
                    { "CTY005", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "Maricopa County", "AZ" },
                    { "CTY006", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "Miami-Dade County", "FL" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Code", "AddressLine1", "AddressLine2", "City", "Country", "County", "CreatedBy", "CreatedDate", "IsActive", "ModifiedBy", "ModifiedDate", "PostalCode", "State" },
                values: new object[,]
                {
                    { "LOC001", "123 Main Street", null, "New York", "USA", "New York County", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "10001", "NY" },
                    { "LOC002", "456 Oak Avenue", null, "Los Angeles", "USA", "Los Angeles County", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "90210", "CA" },
                    { "LOC003", "789 Chicago Blvd", null, "Chicago", "USA", "Cook County", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "60601", "IL" },
                    { "LOC004", "321 Houston Street", null, "Houston", "USA", "Harris County", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, "77001", "TX" }
                });

            migrationBuilder.InsertData(
                table: "ObjectCodes",
                columns: new[] { "Code", "CreatedBy", "CreatedDate", "Description", "IsActive", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { "OBJ001", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desktop computers, laptops, servers", true, null, null, "Computer Equipment" },
                    { "OBJ002", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desks, chairs, filing cabinets", true, null, null, "Office Furniture" },
                    { "OBJ003", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Software applications and licenses", true, null, null, "Software License" },
                    { "OBJ004", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Routers, switches, access points", true, null, null, "Network Equipment" },
                    { "OBJ005", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Phones, tablets, mobile accessories", true, null, null, "Mobile Devices" },
                    { "OBJ006", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cameras, projectors, audio systems", true, null, null, "Audio/Video Equipment" }
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Code", "CreatedBy", "CreatedDate", "Description", "IsActive", "ModifiedBy", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { "ORG001", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IT and Technology Services", true, null, null, "Technology Department" },
                    { "ORG002", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Financial Services and Accounting", true, null, null, "Finance Department" },
                    { "ORG003", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HR and Employee Services", true, null, null, "Human Resources" },
                    { "ORG004", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Operations and Facilities Management", true, null, null, "Operations Department" },
                    { "ORG005", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Legal Services and Compliance", true, null, null, "Legal Department" }
                });

            migrationBuilder.InsertData(
                table: "ReceivableReports",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AttestedBy", "AttestedDate", "City", "CompletedBy", "CompletedDate", "County", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsDeleted", "LocationCode", "ModifiedBy", "ModifiedDate", "OrderStatus", "OrganizationCode", "PostalCode", "State" },
                values: new object[,]
                {
                    { 1, "123 Main Street", null, null, null, "New York", null, null, "New York County", "John Smith", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "LOC001", null, null, "Partial", "ORG001", "10001", "NY" },
                    { 2, "456 Oak Avenue", null, null, null, "Los Angeles", "Jane Doe", new DateTime(2024, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Los Angeles County", "Jane Doe", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "LOC002", null, null, "Complete", "ORG002", "90210", "CA" },
                    { 3, "789 Chicago Blvd", null, null, null, "Chicago", null, null, "Cook County", "Mike Johnson", new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "LOC003", null, null, "Partial", "ORG003", "60601", "IL" },
                    { 4, "321 Houston Street", null, "Admin User", new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Houston", "Sarah Wilson", new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Harris County", "Sarah Wilson", new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "LOC004", null, null, "Complete", "ORG001", "77001", "TX" }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AssetTag", "AssetValue", "AssignedTo", "Brand", "CountyCode", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "Floor", "IsDeleted", "IsOwnedByCounty", "Make", "Model", "ModifiedBy", "ModifiedDate", "ObjectCode", "ReceivableReportId", "Room", "SerialNumber" },
                values: new object[,]
                {
                    { 1, "DL-7420-001", 1200.00m, "John Doe", "Dell", null, "John Smith", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "2", false, false, "Latitude", "7420", null, null, "OBJ001", 1, "201A", "SN123456789" },
                    { 2, "HP-840-001", 1100.00m, "Jane Smith", "HP", "CTY001", "John Smith", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "2", false, true, "EliteBook", "840 G8", null, null, "OBJ001", 1, "202B", "SN987654321" },
                    { 3, "HM-CHAIR-001", 800.00m, "Bob Johnson", "Herman Miller", null, "Jane Doe", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "1", false, false, "Aeron", "Chair", null, null, "OBJ002", 2, "101", "CH123456789" },
                    { 4, "CS-2960X-001", 2500.00m, "Network Team", "Cisco", "CTY002", "Jane Doe", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "B1", false, true, "Catalyst", "2960X", null, null, "OBJ004", 2, "Server Room", "NW987654321" },
                    { 5, "AP-IP14-001", 999.00m, "Marketing Team", "Apple", null, "Mike Johnson", new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "3", false, false, "iPhone", "14 Pro", null, null, "OBJ005", 3, "301", "MB123456789" },
                    { 6, "MS-SL5-001", 1300.00m, "Executive Team", "Microsoft", "CTY004", "Sarah Wilson", new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "4", false, true, "Surface", "Laptop 5", null, null, "OBJ001", 4, "Executive Suite", "SL987654321" },
                    { 7, "CN-PP100-001", 350.00m, "Design Team", "Canon", null, "Sarah Wilson", new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "3", false, false, "PIXMA", "Pro-100", null, null, "OBJ006", 4, "Design Lab", "PR123456789" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistory_AssetId",
                table: "AssetHistory",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistory_HistoryDate",
                table: "AssetHistory",
                column: "HistoryDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistory_OperationType",
                table: "AssetHistory",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTag",
                table: "Assets",
                column: "AssetTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CountyCode",
                table: "Assets",
                column: "CountyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatedDate",
                table: "Assets",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_IsDeleted",
                table: "Assets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ObjectCode",
                table: "Assets",
                column: "ObjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ReceivableReportId",
                table: "Assets",
                column: "ReceivableReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNumber",
                table: "Assets",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Counties_Name_State",
                table: "Counties",
                columns: new[] { "Name", "State" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectCodes_Name",
                table: "ObjectCodes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReportHistory_HistoryDate",
                table: "ReceivableReportHistory",
                column: "HistoryDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReportHistory_OperationType",
                table: "ReceivableReportHistory",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReportHistory_ReceivableReportId",
                table: "ReceivableReportHistory",
                column: "ReceivableReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_CreatedDate",
                table: "ReceivableReports",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_IsDeleted",
                table: "ReceivableReports",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_LocationCode",
                table: "ReceivableReports",
                column: "LocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_OrderStatus",
                table: "ReceivableReports",
                column: "OrderStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_OrganizationCode",
                table: "ReceivableReports",
                column: "OrganizationCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetHistory");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "ReceivableReportHistory");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "ObjectCodes");

            migrationBuilder.DropTable(
                name: "ReceivableReports");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
