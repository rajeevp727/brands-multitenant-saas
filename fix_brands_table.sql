DO $$ 
BEGIN 
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Brands' AND column_name='IsVisible') THEN
        ALTER TABLE "Brands" ADD COLUMN "IsVisible" BOOLEAN DEFAULT TRUE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='Brands' AND column_name='IsActive') THEN
        ALTER TABLE "Brands" ADD COLUMN "IsActive" BOOLEAN DEFAULT TRUE;
    END IF;
END $$;
