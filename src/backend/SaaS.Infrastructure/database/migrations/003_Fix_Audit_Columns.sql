USE MultiTenantSaaS_DB;
GO

-- Fix Users table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'CreatedBy')
    ALTER TABLE Users ADD CreatedBy NVARCHAR(255) DEFAULT 'System';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'LastModifiedAt')
    ALTER TABLE Users ADD LastModifiedAt DATETIME2;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'LastModifiedBy')
    ALTER TABLE Users ADD LastModifiedBy NVARCHAR(255);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsDeleted')
    ALTER TABLE Users ADD IsDeleted BIT DEFAULT 0;
GO

-- Fix Orders table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'CreatedBy')
    ALTER TABLE Orders ADD CreatedBy NVARCHAR(255) DEFAULT 'System';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'LastModifiedAt')
    ALTER TABLE Orders ADD LastModifiedAt DATETIME2;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'LastModifiedBy')
    ALTER TABLE Orders ADD LastModifiedBy NVARCHAR(255);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'IsDeleted')
    ALTER TABLE Orders ADD IsDeleted BIT DEFAULT 0;
GO

-- Fix Payments table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'CreatedBy')
    ALTER TABLE Payments ADD CreatedBy NVARCHAR(255) DEFAULT 'System';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'LastModifiedAt')
    ALTER TABLE Payments ADD LastModifiedAt DATETIME2;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'LastModifiedBy')
    ALTER TABLE Payments ADD LastModifiedBy NVARCHAR(255);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'IsDeleted')
    ALTER TABLE Payments ADD IsDeleted BIT DEFAULT 0;
GO

-- Fix Notifications table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'CreatedBy')
    ALTER TABLE Notifications ADD CreatedBy NVARCHAR(255) DEFAULT 'System';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'LastModifiedAt')
    ALTER TABLE Notifications ADD LastModifiedAt DATETIME2;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'LastModifiedBy')
    ALTER TABLE Notifications ADD LastModifiedBy NVARCHAR(255);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'IsDeleted')
    ALTER TABLE Notifications ADD IsDeleted BIT DEFAULT 0;
GO
