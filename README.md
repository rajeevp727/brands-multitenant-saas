# Multi-Tenant SaaS Platform

This repository contains the core Multi-Tenant SaaS platform along with several business-specific modules.

## 🚀 Quick Start

To launch the entire ecosystem (Core API, Core UI, and all sub-brand modules) in a **single terminal window**, simply run:

```bash
npm start
```

## 📂 Project Structure
...
- `modules/`: Sub-brand implementations:
    - `GreenPantry`: Organic B2C commerce.
    - `OmegaTech`: Technical services and support.
    - `BangaruKottu`: Luxury jewelry commerce.

## 🛠 Management Commands

From the project root:

| Command | Action |
| :--- | :--- |
| `npm start` | Launches all backends and frontends in one terminal. |
| `npm run stop` | Terminate all active `node` and `dotnet` processes. |
| `npm run install-all` | Run `npm install` for all frontend projects. |

## 🔑 Test Credentials (BCrypt Supported)

- **Admin**: `admin@rajeev.com` / `Pass123`
- **Vendor**: `vendor@greenpantry.com` / `Pass123`
- **Customer**: `customer@omega.com` / `Pass123`

---
*Built with ❤️ by the SaaS Platform Team*
