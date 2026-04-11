# MultiTenantSaaS Project Overview

## Direction
This project should run as one multitenant SaaS platform with one shared database only.

Shared production database target:
- `MultiTenantSaaS_DB_PRDDB`

Brand tenants to maintain in the shared platform:
- `rajeev-pvt`
- `greenpantry`
- `omega`
- `bangaru`
- `vanavajram`
- `vajravalli`
- `morebrands`

## Architecture Summary
- `src/backend`: ASP.NET Core + EF Core backend
- `src/frontend`: shared launcher/dashboard UI
- `modules/*`: brand-specific frontend/backend modules
- `database/*`: historical SQL scripts and seed references

The launcher should not create a separate database per brand. Every brand must live inside the same multitenant database and be distinguished by `TenantId` and brand metadata.

## Current Repo Findings
- The shared dashboard already consumes `/api/brands`.
- `ApplicationDbContext` already uses one shared `Tenants` and `Brands` model.
- The current backend is still configured with PostgreSQL (`UseNpgsql`).
- The current initializer mixes EF migrations with raw SQL patch scripts.

## Required DB Rule
Use EF Core Code First only for all database work:
- entity modeling
- migrations
- database creation/update
- seed data

Raw SQL patch scripts should not be the source of truth for schema evolution.

## Brand Dashboard Flow
The root URL `/` should behave as the corporate launcher:
1. Show splash screen as `Rajeev's Tech` with the background image.
2. Open the dashboard page at `/`.
3. Fetch visible brands from the shared `Brands` table through `/api/brands`.
4. Render each brand as a tile.
5. Open the specific brand URL/domain in a new tab when a live tile is clicked.

## Data Model Notes
Shared tables already present in the domain model:
- `Tenants`
- `Brands`
- `Users`
- `Roles`
- `Permissions`
- `FeatureFlags`
- `AppConstants`
- `Products`
- `Orders`
- `OrderItems`
- `Payment`
- `Notifications`
- `AuditLogs`

## Seed Strategy
Seed the shared database through EF Core using deterministic tenant-aware records for:
- one tenant row per brand
- one brand row per tenant
- default roles and permissions
- one platform admin user
- sample app constants and feature flags
- sample products per brand
- sample orders, order items, payments, and notifications

## This Pass
Frontend/navigation changes in scope:
- normalize splash branding to `Rajeev's Tech`
- make `/` the shared dashboard
- show all visible brands as tiles
- open each brand in a new tab
- keep navigation centered on the dashboard

Backend direction in scope:
- keep one shared DB only
- prepare EF Core Code First seeding/migration cleanup around `MultiTenantSaaS_DB_PRDDB`
