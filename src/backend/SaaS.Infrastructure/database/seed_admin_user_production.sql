-- =====================================================
-- Manual Admin User Seeding Script for Production
-- =====================================================
-- Run this in Supabase SQL Editor if admin@rajeev.com doesn't exist
-- Database: Production (Supabase)
-- Purpose: Create admin user for rajeevstech.com login
-- =====================================================

-- Step 1: Ensure rajeev-pvt tenant exists
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Tenants" WHERE "Id" = 'rajeev-pvt') THEN
        INSERT INTO "Tenants" ("Id", "Name", "Identifier", "IsActive", "CreatedAt")
        VALUES ('rajeev-pvt', 'Rajeev''s Pvt. Ltd.', 'admin', true, NOW());
        RAISE NOTICE 'Created tenant: rajeev-pvt';
    ELSE
        RAISE NOTICE 'Tenant rajeev-pvt already exists';
    END IF;
END $$;

-- Step 2: Ensure Admin role exists for rajeev-pvt tenant
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Roles" WHERE "Name" = 'Admin' AND "TenantId" = 'rajeev-pvt') THEN
        INSERT INTO "Roles" ("Id", "Name", "TenantId")
        VALUES (gen_random_uuid(), 'Admin', 'rajeev-pvt');
        RAISE NOTICE 'Created Admin role for rajeev-pvt';
    ELSE
        RAISE NOTICE 'Admin role already exists for rajeev-pvt';
    END IF;
END $$;

-- Step 3: Create or update admin user
DO $$
DECLARE
    admin_role_id UUID;
BEGIN
    -- Get the Admin role ID
    SELECT "Id" INTO admin_role_id 
    FROM "Roles" 
    WHERE "Name" = 'Admin' AND "TenantId" = 'rajeev-pvt' 
    LIMIT 1;

    IF admin_role_id IS NULL THEN
        RAISE EXCEPTION 'Admin role not found for tenant rajeev-pvt';
    END IF;

    -- Check if admin user exists
    IF EXISTS (SELECT 1 FROM "Users" WHERE "Email" = 'admin@rajeev.com' AND "TenantId" = 'rajeev-pvt') THEN
        -- Update existing user
        UPDATE "Users"
        SET "PasswordHash" = 'Pass123',
            "IsActive" = true,
            "RoleId" = admin_role_id,
            "Username" = 'Admin'
        WHERE "Email" = 'admin@rajeev.com' AND "TenantId" = 'rajeev-pvt';
        
        RAISE NOTICE 'Updated existing admin user: admin@rajeev.com';
    ELSE
        -- Create new user
        INSERT INTO "Users" (
            "Id", 
            "Username", 
            "Email", 
            "PasswordHash", 
            "TenantId", 
            "IsActive", 
            "CreatedAt", 
            "CreatedBy", 
            "RoleId"
        )
        VALUES (
            gen_random_uuid(),
            'Admin',
            'admin@rajeev.com',
            'Pass123',
            'rajeev-pvt',
            true,
            NOW(),
            'ManualScript',
            admin_role_id
        );
        
        RAISE NOTICE 'Created new admin user: admin@rajeev.com';
    END IF;
END $$;

-- Step 4: Verify the admin user was created
SELECT 
    u."Id",
    u."Username",
    u."Email",
    u."TenantId",
    u."IsActive",
    r."Name" as "RoleName",
    u."CreatedAt"
FROM "Users" u
LEFT JOIN "Roles" r ON u."RoleId" = r."Id"
WHERE u."Email" = 'admin@rajeev.com' AND u."TenantId" = 'rajeev-pvt';

-- Expected output:
-- Email: admin@rajeev.com
-- Password: Pass123
-- TenantId: rajeev-pvt
-- IsActive: true
-- RoleName: Admin
