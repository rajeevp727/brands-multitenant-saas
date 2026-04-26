-- vanaVajram Seed Script for Supabase
-- Run this in the Supabase SQL Editor

-- 1. Insert into Tenants
INSERT INTO "Tenants" ("Id", "Name", "Hostname", "IsActive", "Identifier", "CreatedAt")
VALUES ('vanavajram', 'vanaVajram', 'vanavajram.com', true, 'vanavajram', NOW())
ON CONFLICT ("Id") DO NOTHING;

-- 2. Insert into Brands
INSERT INTO "Brands" ("Id", "TenantId", "Name", "LogoUrl", "PrimaryColor", "SecondaryColor", "Slogan", "Description", "Email", "Phone", "BuiltBy", "ConfigJson")
VALUES (
    gen_random_uuid(), 
    'vanavajram', 
    'vanaVajram', 
    '/images/vanaVajram/vanaVajram_logo.png', 
    '#064e3b', 
    '#facc15', 
    'Earth''s Purest Gems', 
    'Ethical diamond sourcing and luxury jewelry marketplace with blockchain-verified provenance.', 
    'concierge@vanavajram.com', 
    '+91 77777 66666', 
    'Vajra Tech Labs', 
    '{"domain": "vanavajram.com", "vercel": "vanavajram", "url": "https://vanavajram.com"}'
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
