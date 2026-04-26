# Database Schema Reference

This application uses Database-First approach with Entity Framework Core. The following tables are expected to exist in the SQL Server database:

## Required Tables

### 1. Users
- **UserId** (int, PK, Identity)
- **Email** (nvarchar(255), Required, Unique)
- **PasswordHash** (nvarchar(500), Required)
- **Role** (nvarchar(50), Nullable) - Should be "Vendor" for vendor users
- **FullName** (nvarchar(255), Nullable)
- **CreatedAt** (datetime, Nullable)
- **IsActive** (bit, Default: true)

### 2. Vendors
- **VendorId** (int, PK, Identity)
- **UserId** (int, FK to Users.UserId)
- **VendorName** (nvarchar(255), Required)
- **ContactNumber** (nvarchar(20), Nullable)
- **Address** (nvarchar(500), Nullable)
- **Description** (nvarchar(max), Nullable)
- **IsActive** (bit, Default: true)
- **CreatedAt** (datetime, Nullable)

### 3. Categories
- **CategoryId** (int, PK, Identity)
- **CategoryName** (nvarchar(255), Required)
- **Description** (nvarchar(max), Nullable)
- **IsActive** (bit, Default: true)

### 4. Products
- **ProductId** (int, PK, Identity)
- **VendorId** (int, FK to Vendors.VendorId)
- **CategoryId** (int, FK to Categories.CategoryId)
- **ProductName** (nvarchar(255), Required)
- **Description** (nvarchar(1000), Nullable)
- **Price** (decimal(18,2), Required)
- **StockQuantity** (int, Nullable)
- **IsActive** (bit, Default: true)
- **CreatedAt** (datetime, Nullable)
- **ImageUrl** (nvarchar(max), Nullable)

### 5. Orders
- **OrderId** (int, PK, Identity)
- **VendorId** (int, FK to Vendors.VendorId)
- **OrderStatus** (nvarchar(50), Default: "Pending")
  - Valid values: Pending, Accepted, Preparing, Dispatched, Delivered, Rejected
- **TotalAmount** (decimal(18,2), Required)
- **CustomerName** (nvarchar(255), Nullable)
- **CustomerPhone** (nvarchar(20), Nullable)
- **DeliveryAddress** (nvarchar(500), Nullable)
- **OrderDate** (datetime, Default: GETUTCDATE())
- **AcceptedAt** (datetime, Nullable)
- **PreparedAt** (datetime, Nullable)
- **DispatchedAt** (datetime, Nullable)
- **DeliveredAt** (datetime, Nullable)

### 6. OrderItems
- **OrderItemId** (int, PK, Identity)
- **OrderId** (int, FK to Orders.OrderId)
- **ProductId** (int, FK to Products.ProductId)
- **Quantity** (int, Required)
- **Price** (decimal(18,2), Required)

## Notes

1. **Password Hashing**: The application uses PBKDF2 with SHA256 for password verification. If your existing database uses a different hashing algorithm, you may need to:
   - Update the `AuthService.VerifyPassword` method to match your existing format
   - Or migrate existing passwords to the new format

2. **Products Table**: If the Products table doesn't exist, you'll need to create it. The application requires this table for product management.

3. **Foreign Key Relationships**: Ensure all foreign key relationships are properly set up in the database.

4. **Indexes**: Consider adding indexes on:
   - Users.Email (for login lookups)
   - Vendors.UserId (for vendor lookup by user)
   - Products.VendorId (for vendor product queries)
   - Orders.VendorId (for vendor order queries)
   - Orders.OrderDate (for date-based queries)

## Sample Data Setup

To test the application, ensure you have at least:
- One user with Role = "Vendor"
- One vendor record linked to that user
- At least one category
- (Optional) Sample products and orders

