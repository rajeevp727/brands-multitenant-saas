USE MultiTenantSaaS_DB;
GO

-- 1. Add BuiltBy column if not exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Brands') AND name = 'BuiltBy')
BEGIN
    ALTER TABLE Brands ADD BuiltBy NVARCHAR(255) NULL;
END
GO

-- 2. Update existing brands
UPDATE Brands SET BuiltBy = 'OmegaTechnologies Pvt Ltd.';
GO
