-- Add PhoneNumber column to Users table (PostgreSQL/Supabase)
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "PhoneNumber" VARCHAR(50);
