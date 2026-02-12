# Multi-Tenant SaaS — Project Walkthrough

Complete walkthrough for local development and production deployment with **API on Render.com**, **UI on Vercel**, and **SQL on Supabase**. Localhost uses Supabase for the database.

---

## 1. Architecture Overview

### 1.1 What Lives Where

| Component | Local | Production |
|-----------|--------|------------|
| **SQL database** | Supabase (same as prod) | Supabase |
| **Core SaaS API** | `http://localhost:5114` | Render (e.g. `saas-api.onrender.com`) |
| **GreenPantry API** | `http://localhost:7001` | Render (e.g. `greenpantry-api.onrender.com`) |
| **BangaruKottu Vendor API** | Not in root `npm start` (see below) | Optional on Render |
| **Rajeev’s Pvt Ltd UI** | `http://localhost:5173` | Vercel (e.g. `rajeevs-pvt-ltd.vercel.app`) |
| **GreenPantry UI** | `http://localhost:5174` | Vercel (e.g. `green-pantry-saas.vercel.app`) |
| **OmegaTech UI** | `http://localhost:5175` | Vercel (e.g. `omega-technologies.vercel.app`) |
| **BangaruKottu UI** | `http://localhost:5176` | Vercel (e.g. `bangaru-kottu.vercel.app`) |

### 1.2 Tenant / Brand Mapping

Tenancy is driven by **hostname** (and optionally **header**):

- **Header**: `X-Tenant-Id` (set by each frontend from `tenantMap` in `api.ts`).
- **Fallback**: request host (e.g. `omega-technologies.vercel.app` → resolve to tenant `omega`).

Seeded tenants: `rajeev-pvt`, `greenpantry`, `omega`, `bangaru`.

### 1.3 Who Calls Which API

- **Rajeev’s Pvt Ltd, OmegaTech, BangaruKottu (brand/theming)**  
  - Use the **core SaaS API** (auth, brands, users, notifications, analytics).  
  - Local: `http://localhost:5114` (via `VITE_API_URL` or proxy).  
  - Prod: your Render SaaS API URL.

- **GreenPantry**  
  - Uses its **own GreenPantry API** (restaurants, menu, orders, payments).  
  - Local: `http://localhost:7001`.  
  - Prod: your Render GreenPantry API URL.

- **BangaruKottu**  
  - Has a separate **Vendor.API** (vendor dashboard: products, orders, categories).  
  - Root `npm start` currently **does not** start Vendor.API; only SaaS API + GreenPantry API run.  
  - To run “all APIs” locally you’d add Vendor.API to `start:apis` (e.g. on port 7002) and point BangaruKottu frontend to it where needed.

---

## 2. Local Setup

### 2.1 One-Command Start

From repo root:

```bash
npm start
```

This:

1. Starts **SaaS API** (5114) and **GreenPantry API** (7001).
2. Waits for SaaS API health (`/health`).
3. Starts four UIs: SaaS (5173), GreenPantry (5174), OmegaTech (5175), BangaruKottu (5176).

### 2.2 Database (Supabase)

- **Local and “production” both use Supabase** (PostgreSQL).
- Connection string is in:
  - `src/backend/SaaS.Api/appsettings.json` → `ConnectionStrings:DefaultConnection`
  - `modules/GreenPantry/backend/GreenPantry.API/appsettings.json`
  - `modules/BangaruKottu/backend/Vendor.API/appsettings.json`
- Ensure the same Supabase project (or a dedicated dev project) is used and that:
  - **Connection string** uses the **pooler** URL for serverless (e.g. `*-pooler.supabase.com`) and `Pooling=true`.
  - Schema is created via **EF Core migrations** (under `SaaS.Infrastructure`, GreenPantry, BangaruKottu), not the legacy SQL Server scripts in `database/migrations/` (those are SQL Server–oriented and not used for Postgres).

### 2.3 Root Scripts (package.json)

| Script | Purpose |
|--------|--------|
| `npm start` | Start APIs + all UIs (concurrently) |
| `npm run dev` | Same as `npm start` |
| `npm run stop` | Kill node + dotnet processes (Windows) |
| `npm run install-all` | Install deps for all frontends + OmegaTech |

### 2.4 Frontend API Base URLs (Local)

- **SaaS UI / OmegaTech / BangaruKottu**  
  - `VITE_API_URL` → e.g. `http://localhost:5114/api` (SaaS API).  
  - Some hardcoded fallbacks point to `5114`; for production they should use env.

- **GreenPantry**  
  - `VITE_GREENPANTRY_API` (or `VITE_API_BASE_URL`) → GreenPantry API base, e.g. `http://localhost:7001` so that `/api` calls go to `http://localhost:7001/api`.

---

## 3. Deployment

### 3.1 Render.com (APIs)

- **Config**: `render.yaml` at repo root.
- **Services**:
  - **saas-api**: Core SaaS API (Docker).
  - **greenpantry-api**: GreenPantry API (Docker).

**Critical: Docker build context**

- The SaaS API Dockerfile lives under `src/backend/` and uses paths like `SaaS.Api/`, `SaaS.Application/`, etc., which are relative to **that** directory.
- In `render.yaml`, **dockerContext** must be the backend folder, not the repo root, or the build will fail (COPY cannot find `SaaS.Api/`).

**Recommended fix in `render.yaml`:**

```yaml
# Main SaaS API
- type: web
  name: saas-api
  env: docker
  dockerfilePath: ./src/backend/Dockerfile
  dockerContext: ./src/backend   # was: .
  ...
```

- **Environment variables on Render** (per service):
  - `ASPNETCORE_ENVIRONMENT=Production`
  - `ConnectionStrings__DefaultConnection` = your Supabase connection string (mark as secret).
  - For **saas-api**: `FrontendUrl` = your main UI origin (e.g. `https://rajeevs-pvt-ltd.vercel.app`) for OAuth redirects and CORS.
- **JWT**: In production, override `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` via env (e.g. `Jwt__Issuer=https://saas-api.onrender.com`).

### 3.2 Vercel (UIs)

- Each app is typically a separate Vercel project (or monorepo with multiple roots):
  - **SaaS**: e.g. `src/frontend`
  - **GreenPantry**: `modules/GreenPantry/frontend`
  - **OmegaTech**: `modules/OmegaTech`
  - **BangaruKottu**: `modules/BangaruKottu/frontend`
- **vercel.json** in each app only needs SPA rewrites (e.g. `"source": "/(.*)", "destination": "/index.html"`). You already have this.

**Environment variables on Vercel (build + runtime for Vite):**

- **SaaS / OmegaTech / BangaruKottu** (when they call the core API):
  - `VITE_API_URL` = `https://<your-saas-api>.onrender.com/api`
- **GreenPantry**:
  - `VITE_GREENPANTRY_API` or `VITE_API_BASE_URL` = `https://<your-greenpantry-api>.onrender.com` (no `/api` if you append it in code)
- **Optional**: `VITE_MAIN_PORTAL`, `VITE_GREENPANTRY_UI`, etc., for cross-links between apps.

**CORS**: SaaS API and GreenPantry API use an “allow all” policy and Forwarded Headers for proxies. Ensure `FrontendUrl` and actual Vercel origins are allowed if you later tighten CORS.

### 3.3 Supabase (SQL)

- Use the **connection string** from Supabase (Dashboard → Project Settings → Database):
  - Prefer **Connection pooling** (Session or Transaction mode) for serverless/APIs.
  - Use the **pooler** host (e.g. `*.pooler.supabase.com`) and port (often 5432 or 6543 for pooler).
- **SSL**: `sslmode=Require` (and `Trust Server Certificate=True` if your driver supports it).
- **Secrets**: Never commit real connection strings; use Render/Vercel env and Supabase env vars.
- **Migrations**: Apply EF Core migrations from each backend (SaaS, GreenPantry, BangaruKottu) against this DB (or separate DBs per product if you prefer). The `database/migrations/*.sql` files are SQL Server–oriented; for Postgres, rely on EF migrations.

---

## 4. Validation Checklist

- [ ] **Render**
  - [ ] `dockerContext` for saas-api is `./src/backend`.
  - [ ] `ConnectionStrings__DefaultConnection` set in dashboard for both APIs.
  - [ ] `FrontendUrl` set for saas-api (Vercel URL).
  - [ ] JWT and Google/Facebook auth env vars set for production.
- [ ] **Vercel**
  - [ ] Each frontend has correct `VITE_*` API URL pointing to Render APIs (no localhost).
  - [ ] Build command and output directory are correct (e.g. `npm run build`, `dist`).
- [ ] **Supabase**
  - [ ] Connection string uses pooler and SSL.
  - [ ] Tables created by EF Core migrations; no dependency on SQL Server scripts for runtime.
- [ ] **Local**
  - [ ] Same Supabase connection string in appsettings (or via env) so localhost uses Supabase.
  - [ ] After `npm start`, all four UIs and both APIs start; health checks pass.

---

## 5. Performance and Caveats

### 5.1 Performance

- **Supabase**
  - Use **connection pooling** (pooler URL) to avoid exhausting connections from serverless/APIs.
  - Add indexes for tenant + common filters (e.g. `TenantId`, `TenantId + CreatedAt`).
- **Render (free tier)**
  - Services spin down after inactivity; first request after idle can be slow. Consider a keep-alive (you have `KeepAliveService`) or upgrade if you need stable latency.
- **APIs**
  - Tenant resolution (header/host) and EF global query filters are applied per request; ensure indexes on `TenantId` (and any JSON columns you query).
- **Frontends**
  - Vite builds with chunk splitting (e.g. vendor, ui) in SaaS frontend; consider similar for other apps. Lazy-load routes where possible.

### 5.2 Caveats

1. **BangaruKottu Vendor API**  
   Not started by root `npm start`. To run “all” APIs (Rajeev, GreenPantry, OmegaTech, BangaruKottu), add a third process to `start:apis` for `modules/BangaruKottu/backend/Vendor.API` (e.g. `--urls http://localhost:7002`) and point BangaruKottu frontend’s vendor calls to that base URL when needed.

2. **Hardcoded localhost in frontends**  
   - `BrandThemeSync` in GreenPantry and BangaruKottu uses `http://localhost:5114/api/brands/current`.  
   - For production, these should use the same base URL as the rest of the app (e.g. `VITE_API_URL` or a shared config) so they work on Vercel.

3. **Cookie policy**  
   `CookieSecurePolicy.Always` is set for Render. If you ever run the API over HTTP locally, cookies may not be sent; use HTTPS or relax for Development only.

4. **Google OAuth redirect**  
   `FrontendUrl` must exactly match the Vercel app URL (including `https://`) so redirects after login work. Add the same URL to Google Cloud Console authorized redirect URIs.

5. **database/migrations/**  
   The SQL files there are SQL Server syntax (e.g. `NVARCHAR`, `GETUTCDATE()`). Your runtimes use **PostgreSQL** via EF Core. Use EF migrations for schema; treat the SQL folder as reference or for a different DB if needed.

6. **Multiple APIs and CORS**  
   Both APIs allow all origins. If you lock down CORS later, list all Vercel preview and production origins for each API.

---

## 6. Quick Reference: Ports and URLs

| Service | Port (local) | Env / URL (prod) |
|---------|----------------|------------------|
| SaaS API | 5114 | Render URL → `VITE_API_URL` (e.g. `.../api`) |
| GreenPantry API | 7001 | Render URL → `VITE_GREENPANTRY_API` |
| BangaruKottu Vendor API | 5000 default | Optional; not in root start |
| SaaS UI | 5173 | Vercel |
| GreenPantry UI | 5174 | Vercel |
| OmegaTech UI | 5175 | Vercel |
| BangaruKottu UI | 5176 | Vercel |

---

## 7. Test Credentials (from README)

- **Admin**: `admin@rajeev.com` / `Pass123`
- **Vendor**: `vendor@greenpantry.com` / `Pass123`
- **Customer**: `customer@omega.com` / `Pass123`

Use these against the SaaS or GreenPantry API depending on which app you are testing.

---

*Last updated for Render + Vercel + Supabase setup; localhost pointed at Supabase for SQL.*
