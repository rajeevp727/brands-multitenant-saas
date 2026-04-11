CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "AppConstants" (
    "Id" uuid NOT NULL,
    "TenantId" text,
    "ConstantKey" text NOT NULL,
    "ConstantValue" text NOT NULL,
    "Category" text NOT NULL,
    CONSTRAINT "PK_AppConstants" PRIMARY KEY ("Id")
);

CREATE TABLE "FeatureFlags" (
    "Id" uuid NOT NULL,
    "TenantId" text NOT NULL,
    "FeatureKey" text NOT NULL,
    "IsEnabled" boolean NOT NULL,
    CONSTRAINT "PK_FeatureFlags" PRIMARY KEY ("Id")
);

CREATE TABLE "Notifications" (
    "Id" uuid NOT NULL,
    "TenantId" uuid,
    "BrandName" character varying(100) NOT NULL,
    "TargetRole" character varying(50) NOT NULL,
    "UserId" uuid,
    "Type" character varying(50) NOT NULL,
    "Severity" character varying(20) NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Message" text NOT NULL,
    "IsRead" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id")
);

CREATE TABLE "Permissions" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Module" text NOT NULL,
    CONSTRAINT "PK_Permissions" PRIMARY KEY ("Id")
);

CREATE TABLE "Products" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    "Price" numeric NOT NULL,
    "Category" text NOT NULL,
    "TenantId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id")
);

CREATE TABLE "Roles" (
    "Id" uuid NOT NULL,
    "TenantId" text NOT NULL,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Roles" PRIMARY KEY ("Id")
);

CREATE TABLE "Tenants" (
    "Id" text NOT NULL,
    "Name" text NOT NULL,
    "Hostname" text NOT NULL,
    "IsActive" boolean NOT NULL,
    "Identifier" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Tenants" PRIMARY KEY ("Id")
);

CREATE TABLE "RolePermissions" (
    "PermissionId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    CONSTRAINT "PK_RolePermissions" PRIMARY KEY ("PermissionId", "RoleId"),
    CONSTRAINT "FK_RolePermissions_Permissions_PermissionId" FOREIGN KEY ("PermissionId") REFERENCES "Permissions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RolePermissions_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Username" text NOT NULL,
    "Email" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "RoleId" uuid,
    "IsActive" boolean NOT NULL,
    "TenantId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Users_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("Id")
);

CREATE TABLE "Brands" (
    "Id" uuid NOT NULL,
    "TenantId" text NOT NULL,
    "Name" text NOT NULL,
    "LogoUrl" text NOT NULL,
    "PrimaryColor" text NOT NULL,
    "SecondaryColor" text NOT NULL,
    "Slogan" text NOT NULL,
    "Description" text NOT NULL,
    "Email" text NOT NULL,
    "Phone" text NOT NULL,
    "PrivacyPolicy" text,
    "TermsOfService" text,
    "BuiltBy" text NOT NULL,
    "ConfigJson" text NOT NULL,
    CONSTRAINT "PK_Brands" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Brands_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Orders" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "TotalAmount" numeric NOT NULL,
    "Status" text NOT NULL,
    "TenantId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Orders_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "OrderItems" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "UnitPrice" numeric NOT NULL,
    "TenantId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Payment" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Amount" numeric NOT NULL,
    "Method" text NOT NULL,
    "Status" text NOT NULL,
    "TransactionId" text NOT NULL,
    "TenantId" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Payment" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Payment_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Brands_TenantId" ON "Brands" ("TenantId");

CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");

CREATE INDEX "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");

CREATE INDEX "IX_Orders_UserId" ON "Orders" ("UserId");

CREATE INDEX "IX_Payment_OrderId" ON "Payment" ("OrderId");

CREATE INDEX "IX_RolePermissions_RoleId" ON "RolePermissions" ("RoleId");

CREATE UNIQUE INDEX "IX_Users_Email_TenantId" ON "Users" ("Email", "TenantId");

CREATE INDEX "IX_Users_RoleId" ON "Users" ("RoleId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260130153347_InitialPostgres', '8.0.2');

COMMIT;

START TRANSACTION;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260201133249_AddNewBrandFields', '8.0.2');

COMMIT;

START TRANSACTION;

ALTER TABLE "Users" ADD "City" text;

ALTER TABLE "Users" ADD "Country" text;

ALTER TABLE "Users" ADD "PostalCode" text;

ALTER TABLE "Users" ADD "State" text;

ALTER TABLE "Users" ADD "StreetAddress" text;

ALTER TABLE "Users" ADD "IsEmailVerified" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users" ADD "RefreshToken" text;

ALTER TABLE "Users" ADD "RefreshTokenExpiryTime" timestamp with time zone;

ALTER TABLE "Users" ADD "PasswordResetToken" text;

ALTER TABLE "Users" ADD "PasswordResetTokenExpiryTime" timestamp with time zone;

ALTER TABLE "Users" ADD "Latitude" double precision;

ALTER TABLE "Users" ADD "Longitude" double precision;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203155738_AddAddressFieldsToUser_V2', '8.0.2');

COMMIT;

START TRANSACTION;

ALTER TABLE "Users" ADD "IsEmailVerified" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users" ADD "Latitude" double precision;

ALTER TABLE "Users" ADD "Longitude" double precision;

ALTER TABLE "Users" ADD "PasswordResetToken" text;

ALTER TABLE "Users" ADD "PasswordResetTokenExpiryTime" timestamp with time zone;

ALTER TABLE "Users" ADD "RefreshToken" text;

ALTER TABLE "Users" ADD "RefreshTokenExpiryTime" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203163031_HarmonizeGreenPantryUserFields', '8.0.2');

COMMIT;

START TRANSACTION;

ALTER TABLE "Users" ADD "Role" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203163350_SatisfyGreenPantryRoleV2', '8.0.2');

COMMIT;

START TRANSACTION;

ALTER TABLE "Users" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Products" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Payment" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Orders" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "OrderItems" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260203163641_SatisfyGreenPantryAuditFields', '8.0.2');

COMMIT;

START TRANSACTION;


ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PhoneNumber" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "FirstName" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastName" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "StreetAddress" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "City" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "State" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PostalCode" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Country" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Latitude" double precision;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Longitude" double precision;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "CreatedBy" text NOT NULL DEFAULT 'System';
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastModifiedAt" timestamp with time zone;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastModifiedBy" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean NOT NULL DEFAULT FALSE;
            

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260307174705_ApplyRawSqlUserAddressFields', '8.0.2');

COMMIT;

START TRANSACTION;


ALTER TABLE "Brands" ALTER COLUMN "TenantId" TYPE text USING "TenantId"::text;
ALTER TABLE "Notifications" ALTER COLUMN "TenantId" TYPE text USING "TenantId"::text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Username" text NOT NULL DEFAULT '';
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "TenantId" text NOT NULL DEFAULT '';
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "RoleId" uuid;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PasswordHash" text NOT NULL DEFAULT '';
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "IsEmailVerified" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "FirstName" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastName" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Latitude" double precision;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Longitude" double precision;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PasswordResetToken" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PasswordResetTokenExpiryTime" timestamp with time zone;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "RefreshToken" text;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "RefreshTokenExpiryTime" timestamp with time zone;
ALTER TABLE "Users" ADD COLUMN IF NOT l̥EXISTS "Role" integer NOT NULL DEFAULT 0;
ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "StockQuantity" integer NOT NULL DEFAULT -1;
            

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260405093136_RepairMissingColumns_V4', '8.0.2');

COMMIT;

START TRANSACTION;

ALTER TABLE "Notifications" ALTER COLUMN "TenantId" TYPE text;
UPDATE "Notifications" SET "TenantId" = '' WHERE "TenantId" IS NULL;
ALTER TABLE "Notifications" ALTER COLUMN "TenantId" SET NOT NULL;
ALTER TABLE "Notifications" ALTER COLUMN "TenantId" SET DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260405110644_RepairMissingColumns_V5', '8.0.2');

COMMIT;

