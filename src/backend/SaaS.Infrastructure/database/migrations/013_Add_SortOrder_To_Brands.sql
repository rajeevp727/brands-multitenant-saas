-- Add SortOrder column to Brands table (PostgreSQL/Supabase)
-- Using quoted identifiers to handle case-sensitivity matching likely table definition
ALTER TABLE "Brands" ADD COLUMN IF NOT EXISTS "SortOrder" INT DEFAULT 999;
