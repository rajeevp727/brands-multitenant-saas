-- Add FirstName and LastName columns to Users table (PostgreSQL/Supabase)
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "FirstName" VARCHAR(255);
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastName" VARCHAR(255);
