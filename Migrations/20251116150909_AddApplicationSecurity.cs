using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys only if they exist (they may have been dropped by previous migrations or scripts)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_Counties_CountyCode')
                    ALTER TABLE [Assets] DROP CONSTRAINT [FK_Assets_Counties_CountyCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_ObjectCodes_ObjectCode')
                    ALTER TABLE [Assets] DROP CONSTRAINT [FK_Assets_ObjectCodes_ObjectCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProcurementMethods_ProcurementTypes_ProcurementTypeCode')
                    ALTER TABLE [ProcurementMethods] DROP CONSTRAINT [FK_ProcurementMethods_ProcurementTypes_ProcurementTypeCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceivableReports_Locations_LocationCode')
                    ALTER TABLE [ReceivableReports] DROP CONSTRAINT [FK_ReceivableReports_Locations_LocationCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceivableReports_Organizations_OrganizationCode')
                    ALTER TABLE [ReceivableReports] DROP CONSTRAINT [FK_ReceivableReports_Organizations_OrganizationCode];
            ");

            // Drop Organizations table only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Organizations')
                    DROP TABLE [Organizations];
            ");

            // Drop index only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReceivableReports_OrganizationCode' AND object_id = OBJECT_ID('ReceivableReports'))
                    DROP INDEX [IX_ReceivableReports_OrganizationCode] ON [ReceivableReports];
            ");

            // Drop primary keys only if they exist (they may have been changed by previous migrations)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ProcurementTypes' AND type = 'PK')
                    ALTER TABLE [ProcurementTypes] DROP CONSTRAINT [PK_ProcurementTypes];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ObjectCodes' AND type = 'PK')
                    ALTER TABLE [ObjectCodes] DROP CONSTRAINT [PK_ObjectCodes];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Locations' AND type = 'PK')
                    ALTER TABLE [Locations] DROP CONSTRAINT [PK_Locations];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Counties' AND type = 'PK')
                    ALTER TABLE [Counties] DROP CONSTRAINT [PK_Counties];
            ");

            // Drop indexes only if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_AssetTag' AND object_id = OBJECT_ID('Assets'))
                    DROP INDEX [IX_Assets_AssetTag] ON [Assets];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_CountyCode' AND object_id = OBJECT_ID('Assets'))
                    DROP INDEX [IX_Assets_CountyCode] ON [Assets];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_ObjectCode' AND object_id = OBJECT_ID('Assets'))
                    DROP INDEX [IX_Assets_ObjectCode] ON [Assets];
            ");

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Counties",
                keyColumn: "Code",
                keyValue: "CTY003");

            migrationBuilder.DeleteData(
                table: "Counties",
                keyColumn: "Code",
                keyValue: "CTY005");

            migrationBuilder.DeleteData(
                table: "Counties",
                keyColumn: "Code",
                keyValue: "CTY006");

            migrationBuilder.DeleteData(
                table: "ObjectCodes",
                keyColumn: "Code",
                keyValue: "OBJ003");

            migrationBuilder.DeleteData(
                table: "ProcurementTypes",
                keyColumn: "Code",
                keyValue: "DIRECT");

            migrationBuilder.DeleteData(
                table: "Counties",
                keyColumn: "Code",
                keyValue: "CTY001");

            migrationBuilder.DeleteData(
                table: "Counties",
                keyColumn: "Code",
                keyValue: "CTY002");

            migrationBuilder.DeleteData(
                table: "Counties",
                keyColumn: "Code",
                keyValue: "CTY004");

            migrationBuilder.DeleteData(
                table: "ObjectCodes",
                keyColumn: "Code",
                keyValue: "OBJ001");

            migrationBuilder.DeleteData(
                table: "ObjectCodes",
                keyColumn: "Code",
                keyValue: "OBJ002");

            migrationBuilder.DeleteData(
                table: "ObjectCodes",
                keyColumn: "Code",
                keyValue: "OBJ004");

            migrationBuilder.DeleteData(
                table: "ObjectCodes",
                keyColumn: "Code",
                keyValue: "OBJ005");

            migrationBuilder.DeleteData(
                table: "ObjectCodes",
                keyColumn: "Code",
                keyValue: "OBJ006");

            migrationBuilder.DeleteData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ReceivableReports",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Code",
                keyValue: "LOC001");

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Code",
                keyValue: "LOC002");

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Code",
                keyValue: "LOC003");

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Code",
                keyValue: "LOC004");

            migrationBuilder.DeleteData(
                table: "ProcurementMethods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProcurementMethods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProcurementMethods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProcurementTypes",
                keyColumn: "Code",
                keyValue: "CONTRACT");

            migrationBuilder.DeleteData(
                table: "ProcurementTypes",
                keyColumn: "Code",
                keyValue: "PCARD");

            migrationBuilder.DeleteData(
                table: "ProcurementTypes",
                keyColumn: "Code",
                keyValue: "PO");

            // Drop columns only if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReports') AND name = 'OrganizationCode')
                    ALTER TABLE [ReceivableReports] DROP COLUMN [OrganizationCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'CountyCode')
                    ALTER TABLE [Assets] DROP COLUMN [CountyCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'ObjectCode')
                    ALTER TABLE [Assets] DROP COLUMN [ObjectCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'CountyCode')
                    ALTER TABLE [AssetHistory] DROP COLUMN [CountyCode];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'ObjectCode')
                    ALTER TABLE [AssetHistory] DROP COLUMN [ObjectCode];
            ");

            migrationBuilder.AlterColumn<string>(
                name: "LocationCode",
                table: "ReceivableReports",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            // Add LocationId column only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReports') AND name = 'LocationId')
                BEGIN
                    ALTER TABLE [ReceivableReports] ADD [LocationId] int NULL;
                    
                    -- Populate LocationId from LocationCode if both columns exist
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReports') AND name = 'LocationCode')
                    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Locations') AND name = 'Code')
                    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Locations') AND name = 'Id')
                    BEGIN
                        DECLARE @sql4 NVARCHAR(MAX);
                        SET @sql4 = N'
                            UPDATE rr
                            SET rr.LocationId = l.Id
                            FROM [ReceivableReports] rr
                            INNER JOIN [Locations] l ON rr.LocationCode = l.Code
                            WHERE rr.LocationId IS NULL;
                        ';
                        EXEC sp_executesql @sql4;
                    END
                END
            ");

            // Add Id column to ProcurementTypes only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementTypes') AND name = 'Id')
                BEGIN
                    ALTER TABLE [ProcurementTypes] ADD [Id] int NOT NULL IDENTITY(1,1);
                END
            ");

            migrationBuilder.AlterColumn<string>(
                name: "ProcurementTypeCode",
                table: "ProcurementMethods",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldMaxLength: 10);

            // Add ProcurementTypeId column only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementMethods') AND name = 'ProcurementTypeId')
                BEGIN
                    ALTER TABLE [ProcurementMethods] ADD [ProcurementTypeId] int NULL;
                    
                    -- Populate ProcurementTypeId from ProcurementTypeCode if both columns exist
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementMethods') AND name = 'ProcurementTypeCode')
                    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementTypes') AND name = 'Code')
                    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementTypes') AND name = 'Id')
                    BEGIN
                        DECLARE @sql3 NVARCHAR(MAX);
                        SET @sql3 = N'
                            UPDATE pm
                            SET pm.ProcurementTypeId = pt.Id
                            FROM [ProcurementMethods] pm
                            INNER JOIN [ProcurementTypes] pt ON pm.ProcurementTypeCode = pt.Code
                            WHERE pm.ProcurementTypeId IS NULL;
                        ';
                        EXEC sp_executesql @sql3;
                    END
                END
            ");

            // Add Id column to ObjectCodes only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ObjectCodes') AND name = 'Id')
                BEGIN
                    ALTER TABLE [ObjectCodes] ADD [Id] int NOT NULL IDENTITY(1,1);
                END
            ");

            // Add Id column to Locations only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Locations') AND name = 'Id')
                BEGIN
                    ALTER TABLE [Locations] ADD [Id] int NOT NULL IDENTITY(1,1);
                END
            ");

            // Add Id column to Counties only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Counties') AND name = 'Id')
                BEGIN
                    ALTER TABLE [Counties] ADD [Id] int NOT NULL IDENTITY(1,1);
                END
            ");

            migrationBuilder.AlterColumn<string>(
                name: "AssetTag",
                table: "Assets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            // Add columns to Assets only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetGroupId')
                    ALTER TABLE [Assets] ADD [AssetGroupId] int NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetSubGroupId')
                    ALTER TABLE [Assets] ADD [AssetSubGroupId] int NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'CountyId')
                    ALTER TABLE [Assets] ADD [CountyId] int NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'ObjectCodeId')
                    ALTER TABLE [Assets] ADD [ObjectCodeId] int NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "AssetTag",
                table: "AssetHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            // Add columns to AssetHistory only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'AssetGroupId')
                    ALTER TABLE [AssetHistory] ADD [AssetGroupId] int NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'AssetSubGroupId')
                    ALTER TABLE [AssetHistory] ADD [AssetSubGroupId] int NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'CountyId')
                    ALTER TABLE [AssetHistory] ADD [CountyId] int NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'ObjectCodeId')
                    ALTER TABLE [AssetHistory] ADD [ObjectCodeId] int NULL;
            ");

            // Add primary keys only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ProcurementTypes' AND type = 'PK')
                    ALTER TABLE [ProcurementTypes] ADD CONSTRAINT [PK_ProcurementTypes] PRIMARY KEY ([Id]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ObjectCodes' AND type = 'PK')
                    ALTER TABLE [ObjectCodes] ADD CONSTRAINT [PK_ObjectCodes] PRIMARY KEY ([Id]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Locations' AND type = 'PK')
                    ALTER TABLE [Locations] ADD CONSTRAINT [PK_Locations] PRIMARY KEY ([Id]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Counties' AND type = 'PK')
                    ALTER TABLE [Counties] ADD CONSTRAINT [PK_Counties] PRIMARY KEY ([Id]);
            ");

            // Create ApplicationSecurity table only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApplicationSecurity')
                BEGIN
                    CREATE TABLE [ApplicationSecurity] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Username] nvarchar(255) NOT NULL,
                        [ApplicationRole] nvarchar(50) NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        [IsActive] bit NOT NULL,
                        CONSTRAINT [PK_ApplicationSecurity] PRIMARY KEY ([Id]),
                        CONSTRAINT [CK_ApplicationSecurity_ApplicationRole] CHECK ([ApplicationRole] IN ('Initiator', 'Facility Admin', 'Custodian', 'Delegation'))
                    );
                END
            ");

            // Create AssetGroups table only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AssetGroups')
                BEGIN
                    CREATE TABLE [AssetGroups] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Name] nvarchar(100) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        [IsActive] bit NOT NULL,
                        CONSTRAINT [PK_AssetGroups] PRIMARY KEY ([Id])
                    );
                END
            ");

            // Create AssetSubgroups table only if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AssetSubgroups')
                BEGIN
                    CREATE TABLE [AssetSubgroups] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [ParentAssetGroupId] int NOT NULL,
                        [ChildAssetGroupId] int NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        [IsActive] bit NOT NULL,
                        CONSTRAINT [PK_AssetSubgroups] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_AssetSubgroups_AssetGroups_ChildAssetGroupId] FOREIGN KEY ([ChildAssetGroupId]) REFERENCES [AssetGroups] ([Id]) ON DELETE NO ACTION,
                        CONSTRAINT [FK_AssetSubgroups_AssetGroups_ParentAssetGroupId] FOREIGN KEY ([ParentAssetGroupId]) REFERENCES [AssetGroups] ([Id]) ON DELETE NO ACTION
                    );
                END
            ");

            // Create indexes only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReceivableReports_LocationId' AND object_id = OBJECT_ID('ReceivableReports'))
                    CREATE INDEX [IX_ReceivableReports_LocationId] ON [ReceivableReports] ([LocationId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProcurementTypes_Code' AND object_id = OBJECT_ID('ProcurementTypes'))
                    CREATE UNIQUE INDEX [IX_ProcurementTypes_Code] ON [ProcurementTypes] ([Code]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProcurementMethods_ProcurementTypeId' AND object_id = OBJECT_ID('ProcurementMethods'))
                    CREATE INDEX [IX_ProcurementMethods_ProcurementTypeId] ON [ProcurementMethods] ([ProcurementTypeId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ObjectCodes_Code' AND object_id = OBJECT_ID('ObjectCodes'))
                    CREATE UNIQUE INDEX [IX_ObjectCodes_Code] ON [ObjectCodes] ([Code]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Locations_Code' AND object_id = OBJECT_ID('Locations'))
                    CREATE UNIQUE INDEX [IX_Locations_Code] ON [Locations] ([Code]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Counties_Code' AND object_id = OBJECT_ID('Counties'))
                    CREATE UNIQUE INDEX [IX_Counties_Code] ON [Counties] ([Code]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_AssetGroupId' AND object_id = OBJECT_ID('Assets'))
                    CREATE INDEX [IX_Assets_AssetGroupId] ON [Assets] ([AssetGroupId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_AssetSubGroupId' AND object_id = OBJECT_ID('Assets'))
                    CREATE INDEX [IX_Assets_AssetSubGroupId] ON [Assets] ([AssetSubGroupId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_AssetTag' AND object_id = OBJECT_ID('Assets'))
                    CREATE UNIQUE INDEX [IX_Assets_AssetTag] ON [Assets] ([AssetTag]) WHERE [AssetTag] IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_CountyId' AND object_id = OBJECT_ID('Assets'))
                    CREATE INDEX [IX_Assets_CountyId] ON [Assets] ([CountyId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_ObjectCodeId' AND object_id = OBJECT_ID('Assets'))
                    CREATE INDEX [IX_Assets_ObjectCodeId] ON [Assets] ([ObjectCodeId]);
            ");

            // Create indexes for ApplicationSecurity only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationSecurity_ApplicationRole' AND object_id = OBJECT_ID('ApplicationSecurity'))
                    CREATE INDEX [IX_ApplicationSecurity_ApplicationRole] ON [ApplicationSecurity] ([ApplicationRole]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationSecurity_CreatedDate' AND object_id = OBJECT_ID('ApplicationSecurity'))
                    CREATE INDEX [IX_ApplicationSecurity_CreatedDate] ON [ApplicationSecurity] ([CreatedDate]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationSecurity_IsActive' AND object_id = OBJECT_ID('ApplicationSecurity'))
                    CREATE INDEX [IX_ApplicationSecurity_IsActive] ON [ApplicationSecurity] ([IsActive]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationSecurity_Username' AND object_id = OBJECT_ID('ApplicationSecurity'))
                    CREATE UNIQUE INDEX [IX_ApplicationSecurity_Username] ON [ApplicationSecurity] ([Username]);
            ");

            // Create indexes for AssetGroups and AssetSubgroups only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetGroups_CreatedDate' AND object_id = OBJECT_ID('AssetGroups'))
                    CREATE INDEX [IX_AssetGroups_CreatedDate] ON [AssetGroups] ([CreatedDate]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetGroups_IsActive' AND object_id = OBJECT_ID('AssetGroups'))
                    CREATE INDEX [IX_AssetGroups_IsActive] ON [AssetGroups] ([IsActive]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetGroups_Name' AND object_id = OBJECT_ID('AssetGroups'))
                    CREATE UNIQUE INDEX [IX_AssetGroups_Name] ON [AssetGroups] ([Name]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetSubgroups_ChildAssetGroupId' AND object_id = OBJECT_ID('AssetSubgroups'))
                    CREATE INDEX [IX_AssetSubgroups_ChildAssetGroupId] ON [AssetSubgroups] ([ChildAssetGroupId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetSubgroups_IsActive' AND object_id = OBJECT_ID('AssetSubgroups'))
                    CREATE INDEX [IX_AssetSubgroups_IsActive] ON [AssetSubgroups] ([IsActive]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetSubgroups_ParentAssetGroupId' AND object_id = OBJECT_ID('AssetSubgroups'))
                    CREATE INDEX [IX_AssetSubgroups_ParentAssetGroupId] ON [AssetSubgroups] ([ParentAssetGroupId]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AssetSubgroups_ParentAssetGroupId_ChildAssetGroupId' AND object_id = OBJECT_ID('AssetSubgroups'))
                    CREATE UNIQUE INDEX [IX_AssetSubgroups_ParentAssetGroupId_ChildAssetGroupId] ON [AssetSubgroups] ([ParentAssetGroupId], [ChildAssetGroupId]);
            ");

            // Add foreign keys only if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_AssetGroups_AssetGroupId')
                    ALTER TABLE [Assets] ADD CONSTRAINT [FK_Assets_AssetGroups_AssetGroupId] FOREIGN KEY ([AssetGroupId]) REFERENCES [AssetGroups] ([Id]) ON DELETE SET NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_AssetGroups_AssetSubGroupId')
                    ALTER TABLE [Assets] ADD CONSTRAINT [FK_Assets_AssetGroups_AssetSubGroupId] FOREIGN KEY ([AssetSubGroupId]) REFERENCES [AssetGroups] ([Id]) ON DELETE NO ACTION;
            ");

            // Ensure CountyId and ObjectCodeId are populated before creating foreign keys
            // Use dynamic SQL to avoid column name validation errors
            migrationBuilder.Sql(@"
                -- Populate CountyId from CountyCode if needed (if CountyCode column exists)
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'CountyId')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'CountyCode')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Counties') AND name = 'Code')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Counties') AND name = 'Id')
                BEGIN
                    DECLARE @sql NVARCHAR(MAX);
                    SET @sql = N'
                        UPDATE a
                        SET a.CountyId = c.Id
                        FROM [Assets] a
                        INNER JOIN [Counties] c ON a.CountyCode = c.Code
                        WHERE a.CountyId IS NULL OR a.CountyId NOT IN (SELECT Id FROM [Counties]);
                    ';
                    EXEC sp_executesql @sql;
                END
            ");

            migrationBuilder.Sql(@"
                -- Populate ObjectCodeId from ObjectCode if needed (if ObjectCode column exists)
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'ObjectCodeId')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'ObjectCode')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ObjectCodes') AND name = 'Code')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ObjectCodes') AND name = 'Id')
                BEGIN
                    DECLARE @sql2 NVARCHAR(MAX);
                    SET @sql2 = N'
                        UPDATE a
                        SET a.ObjectCodeId = oc.Id
                        FROM [Assets] a
                        INNER JOIN [ObjectCodes] oc ON a.ObjectCode = oc.Code
                        WHERE a.ObjectCodeId IS NULL OR a.ObjectCodeId NOT IN (SELECT Id FROM [ObjectCodes]);
                    ';
                    EXEC sp_executesql @sql2;
                END
            ");

            // Drop existing foreign keys if they exist (might have invalid data)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_Counties_CountyId')
                    ALTER TABLE [Assets] DROP CONSTRAINT [FK_Assets_Counties_CountyId];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_ObjectCodes_ObjectCodeId')
                    ALTER TABLE [Assets] DROP CONSTRAINT [FK_Assets_ObjectCodes_ObjectCodeId];
            ");

            // Add foreign keys only if data is valid
            migrationBuilder.Sql(@"
                -- Only create CountyId foreign key if all values are valid or NULL
                IF NOT EXISTS (
                    SELECT 1 
                    FROM [Assets] a
                    WHERE a.CountyId IS NOT NULL 
                    AND NOT EXISTS (SELECT 1 FROM [Counties] c WHERE c.Id = a.CountyId)
                )
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_Counties_CountyId')
                    BEGIN
                        ALTER TABLE [Assets] ADD CONSTRAINT [FK_Assets_Counties_CountyId] 
                        FOREIGN KEY ([CountyId]) REFERENCES [Counties] ([Id]) ON DELETE SET NULL;
                    END
                END
            ");

            migrationBuilder.Sql(@"
                -- Only create ObjectCodeId foreign key if all values are valid or NULL
                IF NOT EXISTS (
                    SELECT 1 
                    FROM [Assets] a
                    WHERE a.ObjectCodeId IS NOT NULL 
                    AND NOT EXISTS (SELECT 1 FROM [ObjectCodes] oc WHERE oc.Id = a.ObjectCodeId)
                )
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_ObjectCodes_ObjectCodeId')
                    BEGIN
                        ALTER TABLE [Assets] ADD CONSTRAINT [FK_Assets_ObjectCodes_ObjectCodeId] 
                        FOREIGN KEY ([ObjectCodeId]) REFERENCES [ObjectCodes] ([Id]) ON DELETE SET NULL;
                    END
                END
            ");

            // Ensure ProcurementTypeId is populated before creating foreign key
            migrationBuilder.Sql(@"
                -- Populate ProcurementTypeId from ProcurementTypeCode if needed
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementMethods') AND name = 'ProcurementTypeId')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementMethods') AND name = 'ProcurementTypeCode')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementTypes') AND name = 'Code')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementTypes') AND name = 'Id')
                BEGIN
                    DECLARE @sql5 NVARCHAR(MAX);
                    SET @sql5 = N'
                        UPDATE pm
                        SET pm.ProcurementTypeId = pt.Id
                        FROM [ProcurementMethods] pm
                        INNER JOIN [ProcurementTypes] pt ON pm.ProcurementTypeCode = pt.Code
                        WHERE pm.ProcurementTypeId IS NULL OR pm.ProcurementTypeId NOT IN (SELECT Id FROM [ProcurementTypes]);
                    ';
                    EXEC sp_executesql @sql5;
                END
            ");

            // Drop existing foreign key if it exists (might have invalid data)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId')
                    ALTER TABLE [ProcurementMethods] DROP CONSTRAINT [FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId];
            ");

            // Add foreign key only if data is valid
            migrationBuilder.Sql(@"
                -- Only create the foreign key if all ProcurementTypeId values are valid or NULL
                IF NOT EXISTS (
                    SELECT 1 
                    FROM [ProcurementMethods] pm
                    WHERE pm.ProcurementTypeId IS NOT NULL 
                    AND NOT EXISTS (SELECT 1 FROM [ProcurementTypes] pt WHERE pt.Id = pm.ProcurementTypeId)
                )
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId')
                    BEGIN
                        ALTER TABLE [ProcurementMethods] ADD CONSTRAINT [FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId] 
                        FOREIGN KEY ([ProcurementTypeId]) REFERENCES [ProcurementTypes] ([Id]) ON DELETE NO ACTION;
                    END
                END
            ");

            // Ensure LocationId is populated before creating foreign key
            migrationBuilder.Sql(@"
                -- Populate LocationId from LocationCode if needed
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReports') AND name = 'LocationId')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReports') AND name = 'LocationCode')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Locations') AND name = 'Code')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Locations') AND name = 'Id')
                BEGIN
                    DECLARE @sql6 NVARCHAR(MAX);
                    SET @sql6 = N'
                        UPDATE rr
                        SET rr.LocationId = l.Id
                        FROM [ReceivableReports] rr
                        INNER JOIN [Locations] l ON rr.LocationCode = l.Code
                        WHERE rr.LocationId IS NULL OR rr.LocationId NOT IN (SELECT Id FROM [Locations]);
                    ';
                    EXEC sp_executesql @sql6;
                END
            ");

            // Drop existing foreign key if it exists (might have invalid data)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceivableReports_Locations_LocationId')
                    ALTER TABLE [ReceivableReports] DROP CONSTRAINT [FK_ReceivableReports_Locations_LocationId];
            ");

            // Add foreign key only if data is valid
            migrationBuilder.Sql(@"
                -- Only create the foreign key if all LocationId values are valid or NULL
                IF NOT EXISTS (
                    SELECT 1 
                    FROM [ReceivableReports] rr
                    WHERE rr.LocationId IS NOT NULL 
                    AND NOT EXISTS (SELECT 1 FROM [Locations] l WHERE l.Id = rr.LocationId)
                )
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceivableReports_Locations_LocationId')
                    BEGIN
                        ALTER TABLE [ReceivableReports] ADD CONSTRAINT [FK_ReceivableReports_Locations_LocationId] 
                        FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE SET NULL;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetGroups_AssetGroupId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetGroups_AssetSubGroupId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Counties_CountyId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_ObjectCodes_ObjectCodeId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId",
                table: "ProcurementMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivableReports_Locations_LocationId",
                table: "ReceivableReports");

            migrationBuilder.DropTable(
                name: "ApplicationSecurity");

            migrationBuilder.DropTable(
                name: "AssetSubgroups");

            migrationBuilder.DropTable(
                name: "AssetGroups");

            migrationBuilder.DropIndex(
                name: "IX_ReceivableReports_LocationId",
                table: "ReceivableReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcurementTypes",
                table: "ProcurementTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProcurementTypes_Code",
                table: "ProcurementTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProcurementMethods_ProcurementTypeId",
                table: "ProcurementMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ObjectCodes",
                table: "ObjectCodes");

            migrationBuilder.DropIndex(
                name: "IX_ObjectCodes_Code",
                table: "ObjectCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_Code",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Counties",
                table: "Counties");

            migrationBuilder.DropIndex(
                name: "IX_Counties_Code",
                table: "Counties");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetGroupId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetSubGroupId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetTag",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CountyId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ObjectCodeId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ReceivableReports");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProcurementTypes");

            migrationBuilder.DropColumn(
                name: "ProcurementTypeId",
                table: "ProcurementMethods");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ObjectCodes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Counties");

            migrationBuilder.DropColumn(
                name: "AssetGroupId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetSubGroupId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ObjectCodeId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetGroupId",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "AssetSubGroupId",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "ObjectCodeId",
                table: "AssetHistory");

            migrationBuilder.AlterColumn<string>(
                name: "LocationCode",
                table: "ReceivableReports",
                type: "nchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationCode",
                table: "ReceivableReports",
                type: "nchar(10)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProcurementTypeCode",
                table: "ProcurementMethods",
                type: "nchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "AssetTag",
                table: "Assets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountyCode",
                table: "Assets",
                type: "nchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectCode",
                table: "Assets",
                type: "nchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssetTag",
                table: "AssetHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountyCode",
                table: "AssetHistory",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectCode",
                table: "AssetHistory",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcurementTypes",
                table: "ProcurementTypes",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ObjectCodes",
                table: "ObjectCodes",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Counties",
                table: "Counties",
                column: "Code");

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Code);
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
                    { "ORG001", "System", new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4922), null, true, null, null, "Technology Department" },
                    { "ORG002", "System", new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4924), null, true, null, null, "Finance Department" },
                    { "ORG003", "System", new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4925), null, true, null, null, "Human Resources" },
                    { "ORG004", "System", new DateTime(2025, 10, 9, 2, 7, 6, 36, DateTimeKind.Utc).AddTicks(4926), null, true, null, null, "Operations Department" }
                });

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

            migrationBuilder.InsertData(
                table: "ProcurementMethods",
                columns: new[] { "Id", "ChargeDate", "ContractNumber", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "GroupId", "IsDeleted", "ModifiedBy", "ModifiedDate", "PcardHolderFirstName", "PcardHolderLastName", "ProcurementTypeCode", "PurchaseOrderNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "GRP001", false, null, null, "John", "Smith", "PCARD", null },
                    { 2, null, null, "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, null, null, null, null, "PO", "PO-2024-001" },
                    { 3, null, "CNT-2024-001", "System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, null, null, null, null, "CONTRACT", null }
                });

            migrationBuilder.InsertData(
                table: "ReceivableReports",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "AttestedBy", "AttestedDate", "City", "CompletedBy", "CompletedDate", "County", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "FundId", "IsDeleted", "LocationCode", "ModifiedBy", "ModifiedDate", "OA1Id", "OrderStatus", "OrganizationCode", "OrganizationId", "PostalCode", "ProcurementMethodId", "RRStatus", "State" },
                values: new object[,]
                {
                    { 1, "123 Main Street", null, null, null, "New York", null, null, "New York County", "John Smith", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, false, "LOC001", null, null, 1, "Partial", null, 1, "10001", 1, "Draft", "NY" },
                    { 2, "456 Oak Avenue", null, null, null, "Los Angeles", "Jane Doe", new DateTime(2024, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Los Angeles County", "Jane Doe", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, false, "LOC002", null, null, 2, "Complete", null, 2, "90210", 2, "Draft", "CA" },
                    { 3, "789 Chicago Blvd", null, null, null, "Chicago", null, null, "Cook County", "Mike Johnson", new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 3, false, "LOC003", null, null, 3, "Partial", null, 3, "60601", 3, "Draft", "IL" },
                    { 4, "321 Houston Street", null, "Admin User", new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Houston", "Sarah Wilson", new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Harris County", "Sarah Wilson", new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, false, "LOC004", null, null, 1, "Complete", null, 1, "77001", 1, "Draft", "TX" }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AssetStatus", "AssetTag", "AssetValue", "AssignedTo", "Brand", "CountyCode", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "Floor", "IsDeleted", "IsOwnedByCounty", "Make", "Model", "ModifiedBy", "ModifiedDate", "ObjectCode", "ReceivableReportId", "Room", "SerialNumber", "TagAttestedBy", "TagAttestedDate", "TagPrintedBy", "TagPrintedDate", "UniqueTagNumber" },
                values: new object[,]
                {
                    { 1, "Open", "DL-7420-001", 1200.00m, "John Doe", "Dell", null, "John Smith", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "2", false, false, "Latitude", "7420", null, null, "OBJ001", 1, "201A", "SN123456789", null, null, null, null, null },
                    { 2, "Open", "HP-840-001", 1100.00m, "Jane Smith", "HP", "CTY001", "John Smith", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "2", false, true, "EliteBook", "840 G8", null, null, "OBJ001", 1, "202B", "SN987654321", null, null, null, null, null },
                    { 3, "Open", "HM-CHAIR-001", 800.00m, "Bob Johnson", "Herman Miller", null, "Jane Doe", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "1", false, false, "Aeron", "Chair", null, null, "OBJ002", 2, "101", "CH123456789", null, null, null, null, null },
                    { 4, "Open", "CS-2960X-001", 2500.00m, "Network Team", "Cisco", "CTY002", "Jane Doe", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "B1", false, true, "Catalyst", "2960X", null, null, "OBJ004", 2, "Server Room", "NW987654321", null, null, null, null, null },
                    { 5, "Open", "AP-IP14-001", 999.00m, "Marketing Team", "Apple", null, "Mike Johnson", new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "3", false, false, "iPhone", "14 Pro", null, null, "OBJ005", 3, "301", "MB123456789", null, null, null, null, null },
                    { 6, "Open", "MS-SL5-001", 1300.00m, "Executive Team", "Microsoft", "CTY004", "Sarah Wilson", new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "4", false, true, "Surface", "Laptop 5", null, null, "OBJ001", 4, "Executive Suite", "SL987654321", null, null, null, null, null },
                    { 7, "Open", "CN-PP100-001", 350.00m, "Design Team", "Canon", null, "Sarah Wilson", new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "3", false, false, "PIXMA", "Pro-100", null, null, "OBJ006", 4, "Design Lab", "PR123456789", null, null, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceivableReports_OrganizationCode",
                table: "ReceivableReports",
                column: "OrganizationCode");

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
                name: "IX_Assets_ObjectCode",
                table: "Assets",
                column: "ObjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Counties_CountyCode",
                table: "Assets",
                column: "CountyCode",
                principalTable: "Counties",
                principalColumn: "Code",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_ObjectCodes_ObjectCode",
                table: "Assets",
                column: "ObjectCode",
                principalTable: "ObjectCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcurementMethods_ProcurementTypes_ProcurementTypeCode",
                table: "ProcurementMethods",
                column: "ProcurementTypeCode",
                principalTable: "ProcurementTypes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivableReports_Locations_LocationCode",
                table: "ReceivableReports",
                column: "LocationCode",
                principalTable: "Locations",
                principalColumn: "Code",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivableReports_Organizations_OrganizationCode",
                table: "ReceivableReports",
                column: "OrganizationCode",
                principalTable: "Organizations",
                principalColumn: "Code");
        }
    }
}
