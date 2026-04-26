-- Unified Seed Script to fix vanaVajram and vajraValli
-- Run this in the Supabase SQL Editor

-- ==========================================
-- 1. vanaVajram Fix (Ensure Logo Path)
-- ==========================================

INSERT INTO "Tenants" ("Id", "Name", "Hostname", "IsActive", "Identifier", "CreatedAt")
VALUES ('vanavajram', 'vanaVajram', 'vanavajram.com', true, 'vanavajram', NOW())
ON CONFLICT ("Id") DO NOTHING;

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

-- ==========================================
-- 2. vajraValli Fix (Handle Spelling vajraValli/vajravalli)
-- ==========================================

INSERT INTO "Tenants" ("Id", "Name", "Hostname", "IsActive", "Identifier", "CreatedAt")
VALUES ('vajravalli', 'vajraValli', 'vajravalli.com', true, 'vajravalli', NOW())
ON CONFLICT ("Id") DO NOTHING;

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
    '+91 88888 99999', 
    'Vajra Tech Labs', 
    '{"domain": "vajravalli.com", "vercel": "vajra-valli", "url": "https://vajravalli.com"}'
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
