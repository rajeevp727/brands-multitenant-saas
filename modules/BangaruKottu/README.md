# BangaruKottu Vendor Application

Complete production-ready Vendor Management System with .NET 8 Web API backend and React frontend.

## Architecture

- **Backend**: .NET 8 Web API with Clean Architecture
- **Frontend**: React 18 with Vite, Tailwind CSS
- **Database**: SQL Server (Database-First approach)
- **Authentication**: JWT Bearer Tokens

## Project Structure

```
Vendor/
├── src/
│   ├── Vendor.API/          # Presentation layer (Controllers, Middleware)
│   ├── Vendor.Application/  # Business logic (Services, DTOs, Validators)
│   ├── Vendor.Domain/       # Domain entities
│   └── Vendor.Infrastructure/ # Data access (DbContext, Repositories)
└── frontend/                # React application
```

## Prerequisites

- .NET 8 SDK
- SQL Server (with existing BangaruKottu database)
- Node.js 18+ and npm
- Visual Studio 2022 or VS Code

## Database Setup

1. Ensure SQL Server is running
2. Update connection string in `src/Vendor.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=Rajeevs_BangaruBazar;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

## Backend Setup

1. Navigate to the solution directory:
   ```bash
   cd "D:\Rajeev\Projects\6. Rajeev's SwarnaBazaar\Vendor"
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Update JWT settings in `appsettings.json`:
   ```json
   "Jwt": {
     "Key": "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong",
     "Issuer": "VendorAPI",
     "Audience": "VendorClient"
   }
   ```

4. Run the API:
   ```bash
   cd src/Vendor.API
   dotnet run
   ```

5. API will be available at:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001
   - Swagger: http://localhost:5000/swagger

## Frontend Setup

1. Navigate to frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create `.env` file (optional, defaults to localhost:5000):
   ```env
   VITE_API_URL=http://localhost:5000/api
   ```

4. Run the development server:
   ```bash
   npm run dev
   ```

5. Frontend will be available at: http://localhost:3000

## API Endpoints

### Authentication
- `POST /api/auth/login` - Vendor login

### Vendor Profile
- `GET /api/vendor/profile` - Get vendor profile
- `PUT /api/vendor/profile` - Update vendor profile

### Categories
- `GET /api/category` - Get all categories

### Products
- `GET /api/product` - Get vendor products
- `GET /api/product/{id}` - Get product by ID
- `POST /api/product` - Create product
- `PUT /api/product/{id}` - Update product
- `DELETE /api/product/{id}` - Delete product
- `PATCH /api/product/{id}/toggle-status` - Enable/Disable product

### Orders
- `GET /api/order` - Get vendor orders
- `GET /api/order/{id}` - Get order by ID
- `POST /api/order/{id}/accept` - Accept order
- `POST /api/order/{id}/reject` - Reject order
- `POST /api/order/{id}/prepare` - Mark as preparing
- `POST /api/order/{id}/dispatch` - Dispatch order
- `POST /api/order/{id}/deliver` - Mark as delivered

### Dashboard
- `GET /api/dashboard` - Get dashboard statistics

## Features

### Backend
- ✅ Clean Architecture with separation of concerns
- ✅ Repository pattern for data access
- ✅ JWT authentication and authorization
- ✅ FluentValidation for request validation
- ✅ AutoMapper for DTO mapping
- ✅ Global exception handling
- ✅ Swagger with JWT support
- ✅ CORS configured for React app

### Frontend
- ✅ Modern React 18 with Hooks
- ✅ Tailwind CSS for styling
- ✅ React Router for navigation
- ✅ Axios for API calls
- ✅ Protected routes
- ✅ JWT token management
- ✅ Responsive design (mobile-friendly)
- ✅ Zomato/Swiggy-inspired UI

## Authentication

1. Login with vendor credentials
2. JWT token is stored in localStorage
3. Token is automatically included in API requests
4. Token expires after 8 hours (configurable)

## Order Status Flow

1. **Pending** → Vendor can Accept or Reject
2. **Accepted** → Vendor can Mark as Preparing
3. **Preparing** → Vendor can Dispatch
4. **Dispatched** → Vendor can Mark as Delivered
5. **Delivered** → Order complete

## Notes

- Database schema should already exist (Database-First approach)
- Ensure Users table has vendors with Role = "Vendor"
- Password hashing uses PBKDF2 with SHA256
- All API endpoints require JWT authentication (except login)
- Vendor can only access their own data (enforced by VendorId from JWT)

## Troubleshooting

1. **Connection String Issues**: Verify SQL Server is running and database exists
2. **JWT Errors**: Check JWT Key in appsettings.json is at least 32 characters
3. **CORS Issues**: Verify frontend URL is in CORS policy in Program.cs
4. **404 on API calls**: Check API base URL in frontend `.env` file

## Production Deployment

1. Update connection strings for production database
2. Change JWT Key to a secure random string
3. Set `ASPNETCORE_ENVIRONMENT=Production`
4. Build frontend: `npm run build`
5. Configure reverse proxy (nginx/IIS) for production

---

**Built with ❤️ for BangaruKottu**

