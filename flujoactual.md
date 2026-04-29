# Turnify — Flujo de Avance del Proyecto

> Resumen de cada fase completada con estado y porcentaje de avance global.

---

## Progreso Global

| Fase | Descripción | Estado | % Fase | % Global |
|---|---|---|---|---|
| Fase 0 | Setup del proyecto (solución, docker, CI/CD) | ✅ Completada | 100% | 8% |
| Fase 1 | Core Identity + Tenants (Auth, JWT, registro, login) | ✅ Completada | 100% | 17% |
| Fase 2 | Catalog (servicios, profesionales, sucursales) | ✅ Completada | 100% | 25% |
| Fase 3 | Scheduling + Booking básico (horarios, disponibilidad, citas) | ✅ Completada | 100% | 42% |
| Fase 4 | Notifications (email, recordatorios Hangfire) | ⏳ Pendiente | 0% | 42% |
| Fase 5 | Payments (Wompi, depósitos, reembolsos) | ⏳ Pendiente | 0% | 42% |
| Fase 6 | WhatsApp + SignalR real-time | ⏳ Pendiente | 0% | 42% |
| Fase 7 | Hardening, tests de carga, lanzamiento | ⏳ Pendiente | 0% | 42% |

**Avance total: ~42%**

---

## Fase 0 — Setup ✅ (Semana 1)

**Fecha:** 2026-04-29

### Qué se hizo
- Creada la **solución `.sln`** con 46 proyectos organizados en:
  - `src/Bootstrapper/Turnify.Api` — único proyecto desplegable
  - `src/Modules/{Tenants,Identity,Catalog,Scheduling,Booking,Payments,Notifications,Reporting}` — 4 capas por módulo
  - `src/Shared/{Kernel,Infrastructure,Contracts}` — abstracciones compartidas
  - `tests/{Unit,Integration,Architecture}` — proyectos de test
- **`Directory.Build.props`** con .NET 8, nullable enable, TreatWarningsAsErrors
- **`.editorconfig`** con convenciones C# / JSON / YAML
- **`.gitignore`** estándar .NET (generado con `dotnet new gitignore`)
- **`docker-compose.yml`** con MySQL 8 + Redis 7 + Seq + API, todos con healthchecks
- **`Dockerfile`** multi-stage optimizado para la API
- **`.github/workflows/ci.yml`** con GitHub Actions: build + unit tests + architecture tests + integration tests (con MySQL y Redis reales)

### Resultado
`dotnet build` → 0 errores, 0 advertencias

---

## Fase 1 — Core Identity + Tenants ✅ (Semana 2-3)

**Fecha:** 2026-04-29

### Qué se hizo

#### Shared.Kernel
- `Entity<TId>` / `AggregateRoot<TId>` — clases base con audit fields y domain events
- `DomainEvent` — record base para eventos de dominio
- `IDomainEvent`, `ICurrentTenant`, `IUnitOfWork` — interfaces clave
- `Result<T>` / `Error` — patrón Result para manejo de errores sin excepciones
- `NewUlid.Generate()` — generación de ULIDs para IDs públicos (paquete Cysharp/Ulid)

#### Shared.Infrastructure
- `TurnifyDbContext` — DbContext base que despacha domain events en `SaveChangesAsync` y stampa audit fields automáticamente
- `CurrentTenant` — extrae `tenant_id` y `tenant_slug` del JWT via `IHttpContextAccessor`

#### Shared.Contracts
- `TenantRegisteredEvent` y `UserRegisteredEvent` — eventos públicos inter-módulo

#### Módulo Tenants
- **Domain:** `Tenant` (aggregate root con ULID, slug único, state machine Active/Suspended/Cancelled), `SubscriptionPlan`
- **Application:** `RegisterTenantCommand` + handler + validator, `GetTenantQuery` + handler
- **Infrastructure:** `TenantsDbContext` schema `tenants`, EF configs, seed de planes Starter/Pro/Business
- **Api:** `GET /api/tenants/{publicId}` (requiere auth)
- **Migración:** `InitialCreate` generada

#### Módulo Identity
- **Domain:** `User` (Argon2id, lockout exponencial), `RefreshToken` (rotación + detección de reuso)
- **Application:** Register, Login, RefreshToken handlers con brute-force protection
- **Infrastructure:** `IdentityDbContext` schema `identity`, `Argon2PasswordHasher`, `JwtTokenService` (HS256 15min)
- **Api:** `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh`
- **Migración:** `InitialCreate` generada

### Resultado
`dotnet build` → 0 errores, 0 advertencias

### Deuda técnica
- [ ] Endpoint de verificación de email
- [ ] Endpoint de cambio de contraseña
- [ ] Warning EF: query filter `User` afecta `RefreshToken` — hacer navegación opcional

---

## Fase 2 — Catalog ✅ (Semana 4)

**Fecha:** 2026-04-29

### Qué se hizo

#### Módulo Catalog — Domain
- **`Location`** (AggregateRoot) — sucursales del tenant con ULID, coordenadas GPS opcionales, timezone, soft delete
- **`ServiceCategory`** — agrupación de servicios
- **`Service`** (AggregateRoot) — servicios con duración, buffers antes/después, precio, color HEX, depósito opcional
- **`Staff`** (AggregateRoot) — profesionales con ULID, título profesional, bio, foto, flag `IsBookable`
- **`StaffService`** (M:N) — qué servicios presta cada profesional
- **`StaffLocation`** (M:N) — en qué sucursales trabaja cada profesional

#### Módulo Catalog — Application
- `CreateLocationCommand` + handler + validator (FluentValidation)
- `GetLocationsQuery` + handler (filtra por tenant, solo activos, AsNoTracking)
- `CreateServiceCommand` + handler + validator (validación de HEX color, depósito)
- `GetServicesQuery` + handler (filtra por tenant y activos opcionales)
- `CreateStaffCommand` + handler + validator
- `GetStaffQuery` + handler (incluye StaffServices y StaffLocations)
- `ICatalogDbContext` — interfaz que desacopla Application de Infrastructure

#### Módulo Catalog — Infrastructure
- `CatalogDbContext` schema `catalog` con todos los DbSet
- Configuraciones EF Core: índices compuestos en M:N (`StaffId+ServiceId`, `StaffId+LocationId`), query filters soft delete en `Location`, `Service`, `Staff`, foreign keys con `OnDelete.Cascade`
- `CatalogDbContextFactory` para migraciones sin conexión
- `CatalogModule` — registro de DI y MediatR handlers
- **Migración:** `InitialCreate` generada

#### Módulo Catalog — Api
| Verbo | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/locations` | Listar sucursales del tenant |
| `POST` | `/api/locations` | Crear sucursal |
| `GET` | `/api/services` | Listar servicios activos |
| `POST` | `/api/services` | Crear servicio |
| `GET` | `/api/staff` | Listar profesionales con servicios y sucursales |
| `POST` | `/api/staff` | Crear profesional |

### Resultado
`dotnet build` → 0 errores, 0 advertencias — Migración `InitialCreate` generada para `CatalogDbContext`

### Deuda técnica
- [ ] Endpoints de actualización y desactivación (PUT/DELETE) para Location, Service, Staff

---

## Fase 3 — Scheduling + Booking básico ✅ (Semana 5-6)

**Fecha:** 2026-04-29

### Qué se hizo

#### Catalog — pendientes de Fase 2
- `POST /api/staff/{id}/services` — asignar servicio a profesional (valida duplicado)
- `POST /api/staff/{id}/locations` — asignar sucursal a profesional (valida duplicado)

#### Módulo Scheduling — Domain
- **`StaffSchedule`** — horario semanal recurrente por día (`DayOfWeek`, `StartTime`, `EndTime`, `IsActive`)
- **`StaffTimeOff`** — bloques de ausencia con ULID, rango UTC, motivo opcional
- **`Holiday`** — feriados del tenant con ULID, fecha, nombre, flag `IsRecurring` (recurrente anual)

#### Módulo Scheduling — Application
- `SetStaffScheduleCommand` — upsert completo por semana: activa/desactiva/actualiza cada día
- `GetStaffScheduleQuery` — devuelve sólo slots activos, ordenados por día
- `CreateTimeOffCommand` — valida que `EndsAt > StartsAt` antes de persistir
- `CreateHolidayCommand` — valida duplicado de fecha por tenant
- `GetHolidaysQuery` — filtra por año o trae todos los recurrentes
- `ISchedulingDbContext` — desacopla Application de Infrastructure

#### Módulo Scheduling — Infrastructure
- `SchedulingDbContext` schema `scheduling`
- EF configs: índice único `(StaffId, DayOfWeek)` en schedules, índice único `(TenantId, Date)` en holidays
- `SchedulingDbContextFactory` para migraciones sin conexión
- **Migración:** pendiente de generar

#### Módulo Booking — Domain
- **`Appointment`** (AggregateRoot) — state machine completa:
  ```
  PendingPayment → Confirmed → InProgress → Completed
                ↓
             NoShow / CancelledByCustomer / CancelledByBusiness
  ```
- `AppointmentConfirmedEvent`, `AppointmentCancelledEvent` — domain events para notificaciones futuras
- `AppointmentStatus` enum con los 7 estados del negocio

#### Módulo Booking — Application
- `IBookingDbContext`, `ISchedulingReadContext`, `ICatalogReadContext` — interfaces para desacoplamiento
- `CreateAppointmentCommand` — valida staff bookable, calcula duración + buffer, detecta solapamientos con citas existentes
- `GetAppointmentsQuery` — filtros por staff, fecha y estado
- `GetAvailableSlotsQuery` — algoritmo de disponibilidad en 6 pasos:
  1. Fetch duración + buffer del servicio
  2. Fetch horario recurrente del día (`DayOfWeek`)
  3. Verifica feriados (incluyendo recurrentes anuales por mes+día)
  4. Resta bloques de time-off que se solapen con el día
  5. Resta citas confirmadas/en-progreso que se solapen
  6. Genera slots discretos cada N minutos (default 15)

#### Módulo Booking — Infrastructure
- `BookingDbContext` schema `booking`
- `SchedulingReadAdapter` / `CatalogReadAdapter` — wraps read-only de DbContexts hermanos, aislado a Infrastructure
- `BookingDbContextFactory`
- **Migración:** pendiente de generar

#### Api — nuevos endpoints
| Verbo | Ruta | Auth | Descripción |
|---|---|---|---|
| `PUT` | `/api/staff/{id}/schedule` | ✅ | Reemplaza horario semanal completo |
| `GET` | `/api/staff/{id}/schedule` | ✅ | Obtiene horario activo |
| `POST` | `/api/staff/{id}/time-off` | ✅ | Registra ausencia |
| `GET` | `/api/holidays` | ✅ | Lista feriados (filtrable por `?year=`) |
| `POST` | `/api/holidays` | ✅ | Crea feriado |
| `GET` | `/api/appointments` | ✅ | Lista citas (filtros: staff, fecha, estado) |
| `POST` | `/api/appointments` | ✅ | Crea cita con validación de solapamiento |
| `GET` | `/api/availability` | ❌ público | Slots disponibles para staff+servicio+fecha |

### Resultado
`dotnet build` → 0 errores, 0 advertencias

### Pendiente / deuda técnica
- [ ] Generar y aplicar migraciones de `SchedulingDbContext` y `BookingDbContext`
- [ ] `PATCH /api/appointments/{id}/status` — transiciones de estado manuales
- [ ] Página pública del negocio (SSR por slug de tenant)
- [ ] Cacheo de slots disponibles en Redis (60 s TTL, invalidar en cada booking)

---

## Fase 4 — Notifications ⏳ (Semana 7-8)

### Qué se hará

#### Infraestructura base
- Integrar **Hangfire** con Redis como backing store
- Dashboard de Hangfire en `/hangfire` (solo `SuperAdmin`)
- Política de reintentos con **Polly** (exponential backoff, 3 intentos)

#### Módulo Notifications — Domain & Application
- `NotificationChannel` enum: `Email`, `Whatsapp`, `Sms`
- `Notification` entity — log de notificaciones enviadas con estado (`Pending / Sent / Failed`)
- `SendEmailCommand` + handler — integración con **SendGrid**
- `SendAppointmentReminderCommand` + handler — dispara 24 h antes de cada cita

#### Hangfire Jobs
- `AppointmentReminderJob` — cron cada 5 min: busca citas confirmadas en ventana de 24 h y encola recordatorio
- `CleanupNotificationsJob` — cron diario: elimina logs de notificaciones > 90 días

#### Api
- `GET /api/notifications` — historial de notificaciones del tenant (paginado)

#### Pendientes de Fase 3 que se incluyen
- `PATCH /api/appointments/{id}/status` — endpoint de transición de estado
- Listener de `AppointmentConfirmedEvent` → envía email de confirmación al cliente
- Listener de `AppointmentCancelledEvent` → envía email de cancelación

**Comandos para generar migraciones pendientes antes de iniciar:**
```bash
~/.dotnet/tools/dotnet-ef.exe migrations add InitialCreate --project src/Modules/Scheduling/Turnify.Modules.Scheduling.Infrastructure --startup-project src/Bootstrapper/Turnify.Api --context SchedulingDbContext
~/.dotnet/tools/dotnet-ef.exe migrations add InitialCreate --project src/Modules/Booking/Turnify.Modules.Booking.Infrastructure --startup-project src/Bootstrapper/Turnify.Api --context BookingDbContext
~/.dotnet/tools/dotnet-ef.exe database update --project src/Modules/Scheduling/Turnify.Modules.Scheduling.Infrastructure --startup-project src/Bootstrapper/Turnify.Api --context SchedulingDbContext
~/.dotnet/tools/dotnet-ef.exe database update --project src/Modules/Booking/Turnify.Modules.Booking.Infrastructure --startup-project src/Bootstrapper/Turnify.Api --context BookingDbContext
```

---

## Fase 5 — Payments ⏳ (Semana 9-10)

### Qué se hará

#### Módulo Payments — Domain
- `PaymentTransaction` (AggregateRoot) — ULID, monto, moneda (COP), estado, referencia externa Wompi
- `PaymentStatus` enum: `Pending / Approved / Declined / Voided / Refunded`
- `TransactionApprovedEvent`, `TransactionRefundedEvent` — domain events

#### Módulo Payments — Application
- `InitiatePaymentCommand` — crea transacción y genera URL de pago Wompi
- `HandleWompiWebhookCommand` — procesa el webhook de Wompi, valida firma HMAC
- `RefundTransactionCommand` — solicita reembolso a Wompi y actualiza estado
- `GetTransactionsQuery` — historial de pagos del tenant (paginado)

#### Wompi Integration
- Widget de pago embebido (redirect o modal)
- PSE, Nequi, Bancolombia, tarjeta débito/crédito
- Validación de firma del webhook (`X-Wompi-Signature`)
- Soporte a depósito parcial (definido en `Service.DepositAmount`)

#### Api
| Verbo | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/payments/initiate` | Genera URL de pago Wompi para una cita |
| `POST` | `/api/payments/webhook` | Receptor de webhook Wompi (anónimo) |
| `POST` | `/api/payments/{id}/refund` | Solicita reembolso |
| `GET` | `/api/payments` | Historial de pagos del tenant |

#### Listener de eventos
- `TransactionApprovedEvent` → cambia cita de `PendingPayment` a `Confirmed` + envía email de confirmación

---

## Fase 6 — WhatsApp + SignalR ⏳ (Semana 11-12)

### Qué se hará

#### WhatsApp Business Cloud API
- Envío de recordatorios de cita vía WhatsApp (para planes Pro/Business)
- Templates pre-aprobados: confirmación, recordatorio 24 h, cancelación
- `SendWhatsAppCommand` + handler con Polly retry
- Listener de `AppointmentConfirmedEvent` → WhatsApp si el plan lo permite

#### SignalR — Real-time
- Hub `AppointmentsHub` — notifica cambios de estado de citas en tiempo real al panel de administración
- Hub `AvailabilityHub` — invalida caché de disponibilidad en clientes conectados cuando se crea/cancela una cita
- Autenticación JWT en el handshake del hub

#### Api
| Verbo | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/whatsapp/webhook` | Receptor de mensajes entrantes de WhatsApp |
| WS | `/hubs/appointments` | SignalR hub para estado de citas |
| WS | `/hubs/availability` | SignalR hub para invalidación de disponibilidad |

---

## Fase 7 — Hardening + Lanzamiento ⏳ (Semana 13-14)

### Qué se hará

#### Tests
- Tests de integración con Testcontainers para Scheduling y Booking (disponibilidad, concurrencia)
- Tests de arquitectura (NetArchTest): validar que ningún módulo referencia DbContext de otro módulo directamente en Application
- Tests de carga con k6: simular 100 usuarios concurrentes consultando disponibilidad

#### Seguridad
- Añadir `rate limiting` middleware (ASP.NET Core 8 built-in) por IP y por tenant
- Revisar headers de seguridad (`X-Content-Type-Options`, `X-Frame-Options`, CSP)
- Audit log de acciones críticas (cancelación, reembolso, cambio de plan)

#### Performance
- Cacheo de slots disponibles en Redis (TTL 60 s, invalidación por evento)
- Añadir índice `(tenant_id, starts_at)` en tabla `appointments` para queries de disponibilidad
- Revisar N+1 queries con `dotnet-ef` logging

#### Infraestructura Azure
- App Service (Linux, B2) + MySQL Flexible Server + Redis Cache + Key Vault
- CI/CD completo: GitHub Actions → build → test → push imagen Docker → deploy Azure
- Variables de entorno desde Key Vault via Managed Identity

#### Deuda técnica a saldar
- [ ] Verificación de email (Fase 1)
- [ ] Cambio de contraseña (Fase 1)
- [ ] PUT/DELETE para Location, Service, Staff (Fase 2)
- [ ] Página pública SSR por slug de tenant (Fase 3)
- [ ] Cacheo Redis de disponibilidad (Fase 3)
- [ ] Refactorizar `SchedulingReadAdapter`/`CatalogReadAdapter` a queries MediatR cross-módulo

---

## Comandos de referencia rápida

```bash
# Stack local
docker-compose up -d
dotnet run --project src/Bootstrapper/Turnify.Api

# Build y tests
dotnet build
dotnet test

# Migraciones (patrón genérico)
~/.dotnet/tools/dotnet-ef.exe migrations add <Nombre> \
  --project src/Modules/<Modulo>/Turnify.Modules.<Modulo>.Infrastructure \
  --startup-project src/Bootstrapper/Turnify.Api \
  --context <Modulo>DbContext

~/.dotnet/tools/dotnet-ef.exe database update \
  --project src/Modules/<Modulo>/Turnify.Modules.<Modulo>.Infrastructure \
  --startup-project src/Bootstrapper/Turnify.Api \
  --context <Modulo>DbContext

# Contextos disponibles
# TenantsDbContext | IdentityDbContext | CatalogDbContext
# SchedulingDbContext (migración pendiente) | BookingDbContext (migración pendiente)
```
