-- vajraValli Seed Script for Supabase
-- Run this in the Supabase SQL Editor

-- 1. Insert into Tenants
INSERT INTO "Tenants" ("Id", "Name", "Hostname", "IsActive", "Identifier", "CreatedAt")
VALUES ('vajravalli', 'vajraValli', 'vajravalli.com', true, 'vajravalli', NOW())
ON CONFLICT ("Id") DO NOTHING;

-- 2. Insert into Brands
INSERT INTO "Brands" ("Id", "TenantId", "Name", "LogoUrl", "PrimaryColor", "SecondaryColor", "Slogan", "Description", "Email", "Phone", "BuiltBy", "ConfigJson")
VALUES (
    gen_random_uuid(), 
    'vajravalli', 
    'vajraValli', 
    '/images/vajraValli/vajraValli_logo.png', 
    '#4c1d95', 
    '#ffffff', 
    'Majestic Diamonds', 
    'Curated collection of high-clarity diamonds for global luxury brands.', 
    'info@vajravalli.com', 
    '+91 7032075893', 
    'VajraValli', 
    '{"domain": "vajravalli.in", "vercel": "vajra-valli", "url": "https://vajravalli.in"}'
)
ON CONFLICT ("TenantId") DO UPDATE SET
    "LogoUrl" = EXCLUDED."LogoUrl",
    "ConfigJson" = EXCLUDED."ConfigJson",
    "Description" = EXCLUDED."Description",
    "PrimaryColor" = EXCLUDED."PrimaryColor",
    "SecondaryColor" = EXCLUDED."SecondaryColor",
    "Slogan" = EXCLUDED."Slogan",
    "Email" = EXCLUDED."Email",
    "Phone" = EXCLUDED."Phone",
    "BuiltBy" = EXCLUDED."BuiltBy";
