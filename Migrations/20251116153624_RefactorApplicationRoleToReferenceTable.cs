using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class RefactorApplicationRoleToReferenceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key if it exists (from previous migration)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_AssetGroups_AssetSubGroupId')
                    ALTER TABLE [Assets] DROP CONSTRAINT [FK_Assets_AssetGroups_AssetSubGroupId];
            ");

            // Create ApplicationRoles table first
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApplicationRoles')
                BEGIN
                    CREATE TABLE [ApplicationRoles] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Name] nvarchar(50) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(100) NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        [UpdatedBy] nvarchar(100) NULL,
                        [IsActive] bit NOT NULL,
                        CONSTRAINT [PK_ApplicationRoles] PRIMARY KEY ([Id])
                    );
                END
            ");

            // Create indexes for ApplicationRoles
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationRoles_Name' AND object_id = OBJECT_ID('ApplicationRoles'))
                    CREATE UNIQUE INDEX [IX_ApplicationRoles_Name] ON [ApplicationRoles] ([Name]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationRoles_CreatedDate' AND object_id = OBJECT_ID('ApplicationRoles'))
                    CREATE INDEX [IX_ApplicationRoles_CreatedDate] ON [ApplicationRoles] ([CreatedDate]);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationRoles_IsActive' AND object_id = OBJECT_ID('ApplicationRoles'))
                    CREATE INDEX [IX_ApplicationRoles_IsActive] ON [ApplicationRoles] ([IsActive]);
            ");

            // Seed ApplicationRoles table with default roles
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [ApplicationRoles] WHERE [Name] = 'Initiator')
                    INSERT INTO [ApplicationRoles] ([Name], [Description], [CreatedDate], [CreatedBy], [IsActive])
                    VALUES ('Initiator', 'User who can initiate requests', GETUTCDATE(), 'System', 1);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [ApplicationRoles] WHERE [Name] = 'Facility Admin')
                    INSERT INTO [ApplicationRoles] ([Name], [Description], [CreatedDate], [CreatedBy], [IsActive])
                    VALUES ('Facility Admin', 'Facility administrator with administrative privileges', GETUTCDATE(), 'System', 1);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [ApplicationRoles] WHERE [Name] = 'Custodian')
                    INSERT INTO [ApplicationRoles] ([Name], [Description], [CreatedDate], [CreatedBy], [IsActive])
                    VALUES ('Custodian', 'User responsible for asset custody', GETUTCDATE(), 'System', 1);
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM [ApplicationRoles] WHERE [Name] = 'Delegation')
                    INSERT INTO [ApplicationRoles] ([Name], [Description], [CreatedDate], [CreatedBy], [IsActive])
                    VALUES ('Delegation', 'User with delegated permissions', GETUTCDATE(), 'System', 1);
            ");

            // Add ApplicationRoleId column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationSecurity') AND name = 'ApplicationRoleId')
                BEGIN
                    ALTER TABLE [ApplicationSecurity] ADD [ApplicationRoleId] int NULL;
                END
            ");

            // Migrate data from ApplicationRole (string) to ApplicationRoleId (int)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationSecurity') AND name = 'ApplicationRole')
                AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationSecurity') AND name = 'ApplicationRoleId')
                BEGIN
                    UPDATE asec
                    SET asec.ApplicationRoleId = ar.Id
                    FROM [ApplicationSecurity] asec
                    INNER JOIN [ApplicationRoles] ar ON asec.ApplicationRole = ar.Name
                    WHERE asec.ApplicationRoleId IS NULL;
                END
            ");

            // Make ApplicationRoleId NOT NULL after data migration
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationSecurity') AND name = 'ApplicationRoleId')
                AND (SELECT COUNT(*) FROM [ApplicationSecurity] WHERE [ApplicationRoleId] IS NULL) = 0
                BEGIN
                    -- Set default for any NULL values (shouldn't happen after migration)
                    UPDATE [ApplicationSecurity] SET [ApplicationRoleId] = (SELECT TOP 1 [Id] FROM [ApplicationRoles] WHERE [Name] = 'Initiator')
                    WHERE [ApplicationRoleId] IS NULL;
                    
                    ALTER TABLE [ApplicationSecurity] ALTER COLUMN [ApplicationRoleId] int NOT NULL;
                END
            ");

            // Drop old index and constraint if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationSecurity_ApplicationRole' AND object_id = OBJECT_ID('ApplicationSecurity'))
                    DROP INDEX [IX_ApplicationSecurity_ApplicationRole] ON [ApplicationSecurity];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ApplicationSecurity_ApplicationRole')
                    ALTER TABLE [ApplicationSecurity] DROP CONSTRAINT [CK_ApplicationSecurity_ApplicationRole];
            ");

            // Create index for ApplicationRoleId
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApplicationSecurity_ApplicationRoleId' AND object_id = OBJECT_ID('ApplicationSecurity'))
                    CREATE INDEX [IX_ApplicationSecurity_ApplicationRoleId] ON [ApplicationSecurity] ([ApplicationRoleId]);
            ");

            // Add foreign key constraint
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ApplicationSecurity_ApplicationRoles_ApplicationRoleId')
                BEGIN
                    -- Only create if all ApplicationRoleId values are valid
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM [ApplicationSecurity] asec
                        WHERE asec.ApplicationRoleId IS NOT NULL 
                        AND NOT EXISTS (SELECT 1 FROM [ApplicationRoles] ar WHERE ar.Id = asec.ApplicationRoleId)
                    )
                    BEGIN
                        ALTER TABLE [ApplicationSecurity] ADD CONSTRAINT [FK_ApplicationSecurity_ApplicationRoles_ApplicationRoleId] 
                        FOREIGN KEY ([ApplicationRoleId]) REFERENCES [ApplicationRoles] ([Id]) ON DELETE NO ACTION;
                    END
                END
            ");

            // Drop ApplicationRole column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ApplicationSecurity') AND name = 'ApplicationRole')
                    ALTER TABLE [ApplicationSecurity] DROP COLUMN [ApplicationRole];
            ");

            // Re-add foreign key for Assets if needed
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_AssetGroups_AssetSubGroupId')
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetSubGroupId')
                    AND EXISTS (SELECT * FROM sys.tables WHERE name = 'AssetGroups')
                    BEGIN
                        ALTER TABLE [Assets] ADD CONSTRAINT [FK_Assets_AssetGroups_AssetSubGroupId] 
                        FOREIGN KEY ([AssetSubGroupId]) REFERENCES [AssetGroups] ([Id]) ON DELETE NO ACTION;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationSecurity_ApplicationRoles_ApplicationRoleId",
                table: "ApplicationSecurity");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetGroups_AssetSubGroupId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "ApplicationRoles");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationSecurity_ApplicationRoleId",
                table: "ApplicationSecurity");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "ApplicationSecurity");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationRole",
                table: "ApplicationSecurity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSecurity_ApplicationRole",
                table: "ApplicationSecurity",
                column: "ApplicationRole");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ApplicationSecurity_ApplicationRole",
                table: "ApplicationSecurity",
                sql: "[ApplicationRole] IN ('Initiator', 'Facility Admin', 'Custodian', 'Delegation')");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetGroups_AssetSubGroupId",
                table: "Assets",
                column: "AssetSubGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
