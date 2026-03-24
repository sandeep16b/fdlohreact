-- =============================================
-- Database Migration Script: Add Identity ID Columns to All Tables
-- Description: Adds identity ID columns as primary keys to all reference tables
--              and updates foreign key relationships to use IDs instead of codes
-- =============================================

-- Start transaction to ensure atomicity
BEGIN TRANSACTION;

BEGIN TRY
    -- =============================================
    -- STEP 1: Add ID columns to reference tables
    -- =============================================
    
    -- Add ID column to Organizations table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Organizations') AND name = 'Id')
    BEGIN
        ALTER TABLE Organizations ADD Id INT IDENTITY(1,1);
        
        -- Create temporary unique constraint on new ID column
        ALTER TABLE Organizations ADD CONSTRAINT UC_Organizations_Id UNIQUE (Id);
    END
    
    -- Add ID column to Locations table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Locations') AND name = 'Id')
    BEGIN
        ALTER TABLE Locations ADD Id INT IDENTITY(1,1);
        ALTER TABLE Locations ADD CONSTRAINT UC_Locations_Id UNIQUE (Id);
    END
    
    -- Add ID column to Counties table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Counties') AND name = 'Id')
    BEGIN
        ALTER TABLE Counties ADD Id INT IDENTITY(1,1);
        ALTER TABLE Counties ADD CONSTRAINT UC_Counties_Id UNIQUE (Id);
    END
    
    -- Add ID column to ObjectCodes table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ObjectCodes') AND name = 'Id')
    BEGIN
        ALTER TABLE ObjectCodes ADD Id INT IDENTITY(1,1);
        ALTER TABLE ObjectCodes ADD CONSTRAINT UC_ObjectCodes_Id UNIQUE (Id);
    END
    
    -- Add ID column to ProcurementTypes table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementTypes') AND name = 'Id')
    BEGIN
        ALTER TABLE ProcurementTypes ADD Id INT IDENTITY(1,1);
        ALTER TABLE ProcurementTypes ADD CONSTRAINT UC_ProcurementTypes_Id UNIQUE (Id);
    END

    -- =============================================
    -- STEP 2: Add new foreign key columns to dependent tables
    -- =============================================
    
    -- Add new foreign key columns to ReceivableReports table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReports') AND name = 'LocationId')
        ALTER TABLE ReceivableReports ADD LocationId INT NULL;
    
    -- Add new foreign key columns to ProcurementMethods table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProcurementMethods') AND name = 'ProcurementTypeId')
        ALTER TABLE ProcurementMethods ADD ProcurementTypeId INT NULL;
    
    -- Add new foreign key columns to Assets table
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'ObjectCodeId')
        ALTER TABLE Assets ADD ObjectCodeId INT NULL;
        
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'CountyId')
        ALTER TABLE Assets ADD CountyId INT NULL;

    -- =============================================
    -- STEP 3: Populate new foreign key columns with data
    -- =============================================
    
    -- Update ReceivableReports.LocationId based on LocationCode
    UPDATE rr 
    SET LocationId = l.Id
    FROM ReceivableReports rr
    INNER JOIN Locations l ON rr.LocationCode = l.Code
    WHERE rr.LocationCode IS NOT NULL;
    
    -- Update ProcurementMethods.ProcurementTypeId based on ProcurementTypeCode
    UPDATE pm 
    SET ProcurementTypeId = pt.Id
    FROM ProcurementMethods pm
    INNER JOIN ProcurementTypes pt ON pm.ProcurementTypeCode = pt.Code
    WHERE pm.ProcurementTypeCode IS NOT NULL;
    
    -- Update Assets.ObjectCodeId based on ObjectCode
    UPDATE a 
    SET ObjectCodeId = oc.Id
    FROM Assets a
    INNER JOIN ObjectCodes oc ON a.ObjectCode = oc.Code
    WHERE a.ObjectCode IS NOT NULL;
    
    -- Update Assets.CountyId based on CountyCode
    UPDATE a 
    SET CountyId = c.Id
    FROM Assets a
    INNER JOIN Counties c ON a.CountyCode = c.Code
    WHERE a.CountyCode IS NOT NULL;

    -- =============================================
    -- STEP 4: Drop existing foreign key constraints
    -- =============================================
    
    -- Drop existing foreign key constraints that reference codes
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceivableReports_Locations_LocationCode')
        ALTER TABLE ReceivableReports DROP CONSTRAINT FK_ReceivableReports_Locations_LocationCode;
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProcurementMethods_ProcurementTypes_ProcurementTypeCode')
        ALTER TABLE ProcurementMethods DROP CONSTRAINT FK_ProcurementMethods_ProcurementTypes_ProcurementTypeCode;
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_ObjectCodes_ObjectCode')
        ALTER TABLE Assets DROP CONSTRAINT FK_Assets_ObjectCodes_ObjectCode;
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_Counties_CountyCode')
        ALTER TABLE Assets DROP CONSTRAINT FK_Assets_Counties_CountyCode;

    -- =============================================
    -- STEP 5: Update primary keys to use ID columns
    -- =============================================
    
    -- Drop existing primary key constraints and create new ones with ID columns
    
    -- Organizations table
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Organizations' AND type = 'PK')
    BEGIN
        ALTER TABLE Organizations DROP CONSTRAINT PK_Organizations;
        ALTER TABLE Organizations ADD CONSTRAINT PK_Organizations PRIMARY KEY (Id);
        
        -- Make Code a unique constraint instead of primary key
        IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_Organizations_Code')
            ALTER TABLE Organizations ADD CONSTRAINT UC_Organizations_Code UNIQUE (Code);
    END
    
    -- Locations table
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Locations' AND type = 'PK')
    BEGIN
        ALTER TABLE Locations DROP CONSTRAINT PK_Locations;
        ALTER TABLE Locations ADD CONSTRAINT PK_Locations PRIMARY KEY (Id);
        
        IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_Locations_Code')
            ALTER TABLE Locations ADD CONSTRAINT UC_Locations_Code UNIQUE (Code);
    END
    
    -- Counties table
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Counties' AND type = 'PK')
    BEGIN
        ALTER TABLE Counties DROP CONSTRAINT PK_Counties;
        ALTER TABLE Counties ADD CONSTRAINT PK_Counties PRIMARY KEY (Id);
        
        IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_Counties_Code')
            ALTER TABLE Counties ADD CONSTRAINT UC_Counties_Code UNIQUE (Code);
    END
    
    -- ObjectCodes table
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ObjectCodes' AND type = 'PK')
    BEGIN
        ALTER TABLE ObjectCodes DROP CONSTRAINT PK_ObjectCodes;
        ALTER TABLE ObjectCodes ADD CONSTRAINT PK_ObjectCodes PRIMARY KEY (Id);
        
        IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_ObjectCodes_Code')
            ALTER TABLE ObjectCodes ADD CONSTRAINT UC_ObjectCodes_Code UNIQUE (Code);
    END
    
    -- ProcurementTypes table
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ProcurementTypes' AND type = 'PK')
    BEGIN
        ALTER TABLE ProcurementTypes DROP CONSTRAINT PK_ProcurementTypes;
        ALTER TABLE ProcurementTypes ADD CONSTRAINT PK_ProcurementTypes PRIMARY KEY (Id);
        
        IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_ProcurementTypes_Code')
            ALTER TABLE ProcurementTypes ADD CONSTRAINT UC_ProcurementTypes_Code UNIQUE (Code);
    END

    -- =============================================
    -- STEP 6: Create new foreign key constraints using ID columns
    -- =============================================
    
    -- ReceivableReports -> Locations
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ReceivableReports_Locations_LocationId')
        ALTER TABLE ReceivableReports 
        ADD CONSTRAINT FK_ReceivableReports_Locations_LocationId 
        FOREIGN KEY (LocationId) REFERENCES Locations(Id) ON DELETE SET NULL;
    
    -- ProcurementMethods -> ProcurementTypes
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId')
        ALTER TABLE ProcurementMethods 
        ADD CONSTRAINT FK_ProcurementMethods_ProcurementTypes_ProcurementTypeId 
        FOREIGN KEY (ProcurementTypeId) REFERENCES ProcurementTypes(Id) ON DELETE RESTRICT;
    
    -- Assets -> ObjectCodes
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_ObjectCodes_ObjectCodeId')
        ALTER TABLE Assets 
        ADD CONSTRAINT FK_Assets_ObjectCodes_ObjectCodeId 
        FOREIGN KEY (ObjectCodeId) REFERENCES ObjectCodes(Id) ON DELETE SET NULL;
    
    -- Assets -> Counties
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Assets_Counties_CountyId')
        ALTER TABLE Assets 
        ADD CONSTRAINT FK_Assets_Counties_CountyId 
        FOREIGN KEY (CountyId) REFERENCES Counties(Id) ON DELETE SET NULL;

    -- =============================================
    -- STEP 7: Create indexes for new foreign key columns
    -- =============================================
    
    -- Create indexes for performance
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ReceivableReports_LocationId')
        CREATE NONCLUSTERED INDEX IX_ReceivableReports_LocationId ON ReceivableReports(LocationId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProcurementMethods_ProcurementTypeId')
        CREATE NONCLUSTERED INDEX IX_ProcurementMethods_ProcurementTypeId ON ProcurementMethods(ProcurementTypeId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_ObjectCodeId')
        CREATE NONCLUSTERED INDEX IX_Assets_ObjectCodeId ON Assets(ObjectCodeId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_CountyId')
        CREATE NONCLUSTERED INDEX IX_Assets_CountyId ON Assets(CountyId);

    -- =============================================
    -- STEP 8: Update History Tables
    -- =============================================
    
    -- Add new columns to ReceivableReportHistory
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ReceivableReportHistory') AND name = 'LocationId')
        ALTER TABLE ReceivableReportHistory ADD LocationId INT NULL;
    
    -- Add new columns to AssetHistory
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'ObjectCodeId')
        ALTER TABLE AssetHistory ADD ObjectCodeId INT NULL;
        
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AssetHistory') AND name = 'CountyId')
        ALTER TABLE AssetHistory ADD CountyId INT NULL;

    -- =============================================
    -- STEP 9: Clean up temporary constraints
    -- =============================================
    
    -- Remove temporary unique constraints
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_Organizations_Id')
        ALTER TABLE Organizations DROP CONSTRAINT UC_Organizations_Id;
        
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_Locations_Id')
        ALTER TABLE Locations DROP CONSTRAINT UC_Locations_Id;
        
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_Counties_Id')
        ALTER TABLE Counties DROP CONSTRAINT UC_Counties_Id;
        
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_ObjectCodes_Id')
        ALTER TABLE ObjectCodes DROP CONSTRAINT UC_ObjectCodes_Id;
        
    IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'UC_ProcurementTypes_Id')
        ALTER TABLE ProcurementTypes DROP CONSTRAINT UC_ProcurementTypes_Id;

    -- =============================================
    -- STEP 10: Verification Queries
    -- =============================================
    
    PRINT 'Migration completed successfully!';
    PRINT 'Verification:';
    PRINT 'Organizations count: ' + CAST((SELECT COUNT(*) FROM Organizations) AS VARCHAR(10));
    PRINT 'Locations count: ' + CAST((SELECT COUNT(*) FROM Locations) AS VARCHAR(10));
    PRINT 'Counties count: ' + CAST((SELECT COUNT(*) FROM Counties) AS VARCHAR(10));
    PRINT 'ObjectCodes count: ' + CAST((SELECT COUNT(*) FROM ObjectCodes) AS VARCHAR(10));
    PRINT 'ProcurementTypes count: ' + CAST((SELECT COUNT(*) FROM ProcurementTypes) AS VARCHAR(10));
    
    -- Verify foreign key relationships
    PRINT 'ReceivableReports with LocationId: ' + CAST((SELECT COUNT(*) FROM ReceivableReports WHERE LocationId IS NOT NULL) AS VARCHAR(10));
    PRINT 'ProcurementMethods with ProcurementTypeId: ' + CAST((SELECT COUNT(*) FROM ProcurementMethods WHERE ProcurementTypeId IS NOT NULL) AS VARCHAR(10));
    PRINT 'Assets with ObjectCodeId: ' + CAST((SELECT COUNT(*) FROM Assets WHERE ObjectCodeId IS NOT NULL) AS VARCHAR(10));
    PRINT 'Assets with CountyId: ' + CAST((SELECT COUNT(*) FROM Assets WHERE CountyId IS NOT NULL) AS VARCHAR(10));

    -- Commit transaction if everything succeeded
    COMMIT TRANSACTION;
    PRINT 'Transaction committed successfully!';

END TRY
BEGIN CATCH
    -- Rollback transaction if any error occurred
    ROLLBACK TRANSACTION;
    
    PRINT 'Error occurred during migration:';
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Transaction rolled back.';
    
    -- Re-throw the error
    THROW;
END CATCH;

-- =============================================
-- Post-Migration Notes:
-- =============================================
-- 1. The old Code-based columns are kept for backward compatibility
-- 2. New ID-based foreign key relationships are established
-- 3. Unique constraints ensure Code values remain unique
-- 4. History tables are updated to include new ID columns
-- 5. Indexes are created for performance optimization
-- =============================================
