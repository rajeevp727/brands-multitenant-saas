using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaaS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplyRawSqlUserAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""PhoneNumber"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""FirstName"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""LastName"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""StreetAddress"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""City"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""State"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""PostalCode"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""Country"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""Latitude"" double precision;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""Longitude"" double precision;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""CreatedAt"" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""CreatedBy"" text NOT NULL DEFAULT 'System';
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""LastModifiedAt"" timestamp with time zone;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""LastModifiedBy"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""UpdatedAt"" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""IsDeleted"" boolean NOT NULL DEFAULT FALSE;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // We ignore down migrations for raw sql fixes of this nature
        }
    }
}
