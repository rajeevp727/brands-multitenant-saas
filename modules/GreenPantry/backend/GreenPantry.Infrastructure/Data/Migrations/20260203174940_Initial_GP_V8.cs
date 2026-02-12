using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenPantry.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial_GP_V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "greenpantry");

            migrationBuilder.CreateTable(
                name: "MenuItems",
                schema: "greenpantry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    IsVegetarian = table.Column<bool>(type: "boolean", nullable: false),
                    IsVegan = table.Column<bool>(type: "boolean", nullable: false),
                    IsGlutenFree = table.Column<bool>(type: "boolean", nullable: false),
                    IsSpicy = table.Column<bool>(type: "boolean", nullable: false),
                    SpiceLevel = table.Column<int>(type: "integer", nullable: false),
                    Allergens = table.Column<List<string>>(type: "text[]", nullable: false),
                    Ingredients = table.Column<List<string>>(type: "text[]", nullable: false),
                    PreparationTime = table.Column<int>(type: "integer", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Variants = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "greenpantry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DeliveryAddress_Street = table.Column<string>(type: "text", nullable: false),
                    DeliveryAddress_City = table.Column<string>(type: "text", nullable: false),
                    DeliveryAddress_State = table.Column<string>(type: "text", nullable: false),
                    DeliveryAddress_PostalCode = table.Column<string>(type: "text", nullable: false),
                    DeliveryAddress_Country = table.Column<string>(type: "text", nullable: false),
                    DeliveryAddress_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryAddress_Longitude = table.Column<double>(type: "double precision", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentProvider = table.Column<int>(type: "integer", nullable: true),
                    PaymentId = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    UPIQRCode = table.Column<string>(type: "text", nullable: false),
                    PaymentExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstimatedDeliveryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryInstructions = table.Column<string>(type: "text", nullable: false),
                    DeliveryPersonId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Items = table.Column<string>(type: "jsonb", nullable: true),
                    StatusHistory = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                schema: "greenpantry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Address_Street = table.Column<string>(type: "text", nullable: true),
                    Address_City = table.Column<string>(type: "text", nullable: true),
                    Address_State = table.Column<string>(type: "text", nullable: true),
                    Address_PostalCode = table.Column<string>(type: "text", nullable: true),
                    Address_Country = table.Column<string>(type: "text", nullable: true),
                    Address_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CuisineTypes = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EstimatedDeliveryTime = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "greenpantry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    ProviderTransactionId = table.Column<string>(type: "text", nullable: false),
                    ProviderOrderId = table.Column<string>(type: "text", nullable: false),
                    UPIQRCode = table.Column<string>(type: "text", nullable: false),
                    UPIQRData = table.Column<string>(type: "text", nullable: false),
                    QRType = table.Column<int>(type: "integer", nullable: false),
                    QRGeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QRExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WebhookId = table.Column<string>(type: "text", nullable: false),
                    FailureReason = table.Column<string>(type: "text", nullable: false),
                    RefundId = table.Column<string>(type: "text", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RefundedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefundReason = table.Column<string>(type: "text", nullable: false),
                    ProviderMetadata = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Webhooks = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "greenpantry",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                schema: "greenpantry",
                table: "Payments",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems",
                schema: "greenpantry");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "greenpantry");

            migrationBuilder.DropTable(
                name: "Restaurants",
                schema: "greenpantry");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "greenpantry");
        }
    }
}
