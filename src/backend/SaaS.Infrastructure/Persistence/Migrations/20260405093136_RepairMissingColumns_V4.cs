using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaaS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RepairMissingColumns_V4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE ""Brands"" ALTER COLUMN ""TenantId"" TYPE text USING ""TenantId""::text;
ALTER TABLE ""Notifications"" ALTER COLUMN ""TenantId"" TYPE text USING ""TenantId""::text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""Username"" text NOT NULL DEFAULT '';
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""TenantId"" text NOT NULL DEFAULT '';
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""RoleId"" uuid;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""PasswordHash"" text NOT NULL DEFAULT '';
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""IsEmailVerified"" boolean NOT NULL DEFAULT FALSE;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""FirstName"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""LastName"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""Latitude"" double precision;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""Longitude"" double precision;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""PasswordResetToken"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""PasswordResetTokenExpiryTime"" timestamp with time zone;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""RefreshToken"" text;
ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""RefreshTokenExpiryTime"" timestamp with time zone;
ALTER TABLE ""Users"" ADD COLUMN IF NOT l̥EXISTS ""Role"" integer NOT NULL DEFAULT 0;
ALTER TABLE ""Products"" ADD COLUMN IF NOT EXISTS ""StockQuantity"" integer NOT NULL DEFAULT -1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
