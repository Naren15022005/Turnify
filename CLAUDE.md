# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Turnify** is a multi-tenant SaaS booking platform for Colombian service businesses (clinics, barbershops, spas, gyms). The project is currently in the planning/architecture phase — `logica.md` is the authoritative architecture document. No source code exists yet.

## Planned Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 8, ASP.NET Core Web API, MediatR, FluentValidation, Mapster |
| Frontend | Blazor United (.NET 8), MudBlazor |
| ORM | Entity Framework Core 8 + Pomelo (MySQL driver) |
| Database | MySQL 8.0+ |
| Cache | Redis 7 |
| Background Jobs | Hangfire + Redis |
| Real-time | SignalR |
| Logging | Serilog + Seq (dev) / Application Insights (prod) |
| Testing | xUnit + FluentAssertions + Testcontainers |
| Infra | Azure (App Service, MySQL Flexible, Redis, Key Vault, CDN) |
| Payments | Wompi (PSE, Nequi, Bancolomdia, tarjeta) |
| Notifications | WhatsApp Business Cloud API, SendGrid, Twilio |

## Commands (once code exists)

```bash
# Run full stack locally (Docker Compose: API + MySQL + Redis + Seq)
docker-compose up

# Backend
dotnet build
dotnet run --project src/Bootstrapper/Turnify.Api

# Run all tests
dotnet test

# Run a single test project
dotnet test src/Modules/Booking/Booking.UnitTests

# EF Core migrations
dotnet ef migrations add <Name> --project src/Modules/<Module>/Infrastructure
dotnet ef database update

# Secrets (development)
dotnet user-secrets set "ConnectionStrings:Default" "..."
```

## Architecture

### Structure: Modular Monolith with Vertical Slices

Single deployable, module boundaries enforced — modules communicate only via MediatR, never via direct table access.

```
src/
├── Bootstrapper/
│   └── Turnify.Api/              # Entry point, DI composition root
├── Modules/
│   ├── Tenants/                  # Orgs, subscription plans
│   ├── Identity/                 # Auth, users, RBAC, JWT
│   ├── Catalog/                  # Services, professionals, locations
│   ├── Scheduling/               # Recurring schedules, time-off, holidays
│   ├── Booking/                  # Appointments — core domain
│   ├── Payments/                 # Wompi integration, transactions
│   ├── Notifications/            # Email, WhatsApp, SMS
│   └── Reporting/                # Analytics
└── Shared/
    ├── Kernel/                   # Base types, Result<T>, domain event interfaces
    ├── Infrastructure/           # EF base context, common auth middleware
    └── Contracts/                # Domain events shared between modules
```

Each module follows Clean Architecture:
```
<Module>/
├── Domain/         # Entities, value objects, domain events
├── Application/    # CQRS handlers (MediatR), validators, interfaces
├── Infrastructure/ # EF DbContext, repositories, external service clients
└── Api/            # Minimal API endpoints, request/response models
```

### Inter-Module Communication

- **Sync:** MediatR `IRequest<T>` (Query/Command)
- **Async:** MediatR `INotification` + Domain Events via `Shared/Contracts`
- **Prohibited:** Modules must never reference another module's DbContext or tables directly

### Multi-Tenancy

Every business table has `tenant_id BIGINT UNSIGNED NOT NULL`. EF Core global query filters enforce isolation automatically. Middleware extracts `tenantId` from JWT and injects `ICurrentTenant`. **Never bypass global filters.**

### Authentication

- JWT RS256 (asymmetric keys, rotatable). Access token: 15 min. Refresh token: hashed (SHA-256) in DB, rotatable with reuse detection.
- Password hashing: **Argon2id** (not PBKDF2 or bcrypt).
- Roles: `SuperAdmin`, `TenantOwner`, `TenantAdmin`, `Staff`, `Customer`.
- Endpoints use policy-based authorization: `[Authorize(Policy = "CanManageAppointments")]`.
- Brute-force protection: 5 failed logins → 15 min lockout with exponential backoff.
- 2FA (TOTP) mandatory for `TenantOwner` on Business+ plans.

### Database Key Decisions

- **PKs:** `BIGINT` for high-volume internal tables; `CHAR(26)` ULID for URL-exposed entities.
- **Never expose auto-increment IDs** in API responses (IDOR prevention).
- Soft deletes via `is_deleted` flag (no hard deletes on business data).
- All timestamps stored in **UTC**; converted to tenant timezone at application layer.
- Charset: `utf8mb4`, collation: `utf8mb4_0900_ai_ci`.

### Critical Business Logic

**Availability Calculation** (performance-critical):
1. Fetch service duration + buffer times
2. Fetch staff recurring schedule for date range
3. Subtract time-off, holidays
4. Subtract confirmed/in-progress appointments
5. Generate discrete time slots (e.g., every 15 min)
6. Apply tenant rules (min/max booking anticipation)
- Cached in Redis with 60s TTL; invalidated on any booking change

**Concurrent Booking Safety:**
- `SELECT ... FOR UPDATE` or `SERIALIZABLE` isolation on appointment insert
- Re-verify no overlap inside the transaction before committing
- DB unique constraint on `(staff_id, start_at)` as final defense

**Appointment State Machine:**
```
pending_payment → confirmed → in_progress → completed
                ↓
             no_show / cancelled_by_customer / cancelled_by_business
```

**Reminders:** Hangfire job every 5 min finds appointments confirmed ~24h ahead → sends via WhatsApp (if plan allows) or email. Uses Polly exponential backoff for retries.

### Frontend Rendering Strategy

| Page type | Render mode |
|---|---|
| Landing / marketing | Static SSR |
| Booking flow | Interactive Server |
| Admin panel | Interactive WebAssembly |
| Mobile staff view | WebAssembly + PWA |

### Testing Strategy

- **Unit tests:** Per module, business logic in `Application/` and `Domain/`
- **Integration tests:** Testcontainers spins real MySQL instance — no mocking the database
- **Architecture tests:** NetArchTest enforces module boundary rules (no cross-module DbContext access)

## Secrets Management

- **Development:** `dotnet user-secrets`
- **Production:** Azure Key Vault + Managed Identity
- Zero secrets committed to the repository — ever.

## Performance Targets

- API p95 < 200 ms
- API p99 < 500 ms
- Availability endpoint: cached response < 50 ms
