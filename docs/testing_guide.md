# Testing Guide: Multi-Tenant SaaS

## 1. Backend API (Swagger)
The backend is running at `https://localhost:5114` (or `http://localhost:5113`).
- **Swagger URL**: `https://localhost:5114/swagger/index.html`
- **Global Roles**: Every brand now has `Admin`, `Customer`, `Vendor`, and `Delivery Partner` roles seeded.
- **Role-Permissions**: Permissions like `Products.View` and `Orders.Create` are mapped to these roles in the DB.
- **Feature Flags**: Tested via the `FeatureFlags` table (e.g., `EnableAnalytics` for Omega).
- **Multi-Tenancy Test**:
  1. Open Swagger.
  2. Navigate to `GET /api/Products`.
  3. Click "Try it out".
  4. Add the header `X-Tenant-Id: greenpantry` and click Execute. You should see "Organic Apples".
  5. Change the header to `X-Tenant-Id: rajeev-pvt`. You should see an empty list (no products seeded for corporate yet).
  6. Change the header to `X-Tenant-Id: bangaru`. You should see "Gold Necklace".

## 2. Frontend PWA
The frontend is built with Vite and React.
- **Run Frontend**:
  ```bash
  cd src/frontend
  npm run dev
  ```
- **Test Brand Switching**:
  - By default, it detects `localhost` as **Rajeev’s Pvt. Ltd**.
  - To test other brands, you can edit `src/frontend/src/providers/BrandProvider.tsx` or spoof your hostname.
  - The frontend automatically injects the correct `X-Tenant-Id` header into all API calls based on the detected brand.

## 3. Database
- **Database Name**: `MultiTenantSaaS_DB`
- **Seeded Brands**: `rajeev-pvt`, `greenpantry`, `omega`, `bangaru`.
- **Seeded Products**: Available for `greenpantry`, `omega`, and `bangaru`.

### Important Note on Audit Columns
All business entities (`Product`, `User`, `Order`, `Payment`) now fully support auditing. Database tables have been updated with `CreatedBy`, `IsDeleted`, `LastModifiedAt`, and `LastModifiedBy` columns to match the enterprise-grade C# models.
