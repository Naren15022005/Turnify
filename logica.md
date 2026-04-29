# Turnify — Documento Maestro del Proyecto
**SaaS de Reservas y Turnos para Negocios de Servicios en Colombia**

> Documento de arquitectura, lógica de negocio y decisiones técnicas — Fase inicial (MVP)

---

## 1. Resumen ejecutivo

**Turnify** es una plataforma SaaS multitenant donde negocios de servicios (clínicas, consultorios, barberías, peluquerías, gimnasios, spas, talleres) gestionan su agenda, sus clientes y sus pagos, y exponen una página pública donde sus clientes finales reservan citas online sin necesidad de instalar nada.

**Modelo de monetización:** suscripción mensual por negocio, con planes escalonados según número de profesionales, sucursales y funcionalidades.

**Diferenciador en Colombia:** integración nativa con **Wompi** (pagos), **WhatsApp Business** (notificaciones), y soporte real para el flujo informal de pagos (Nequi, Bancolombia, comprobante).

---

## 2. El problema que soluciona

### 2.1 Dolor real del mercado

En Colombia, los negocios pequeños y medianos de servicios gestionan sus citas con métodos primitivos:

| Método actual | Problemas que genera |
|---|---|
| WhatsApp manual | El dueño/recepcionista pierde 2–4 horas diarias coordinando, hay solapamientos, citas olvidadas, no hay historial. |
| Cuaderno físico | Cero respaldo, imposible analizar, no hay recordatorios. |
| Excel compartido | Sin acceso multi-usuario real, errores de sobreescritura, sin acceso desde celular. |
| Calendly / herramientas extranjeras | UX en inglés, sin Wompi/PSE/Nequi, sin WhatsApp colombiano, precios en USD. |

### 2.2 Consecuencias medibles

- **No-shows** (clientes que no llegan): 20–30% sin sistema de recordatorios → pérdida directa de ingresos.
- **Tiempo administrativo:** un negocio de 3 profesionales gasta ~15 horas/semana en coordinación manual.
- **Pérdida de clientes:** sin recordatorios ni historial, la recompra cae.
- **Sin datos:** el dueño no sabe cuál servicio es más rentable, qué profesional tiene más demanda, ni qué horarios sobran.

### 2.3 Lo que Turnify resuelve

1. **Para el negocio:** agenda centralizada, recordatorios automáticos, página pública de reservas, reportes, gestión de pagos.
2. **Para el cliente final:** reservar en 30 segundos sin descargar nada, recordatorios por WhatsApp, historial.
3. **Para el profesional:** ver su día, marcar disponibilidad, bloquear horarios.

---

## 3. Propuesta de valor por segmento

| Segmento objetivo (Fase 1) | Por qué | Ticket esperado COP/mes |
|---|---|---|
| Consultorios médicos pequeños (1–5 profesionales) | Alto valor por cita, dolor agudo de coordinación, dispuestos a pagar | $150.000 – $500.000 |
| Barberías y peluquerías premium | Volumen alto de citas, dueños jóvenes con afinidad digital | $80.000 – $200.000 |
| Spas y centros de estética | Alto valor por cita, clientela fidelizable | $120.000 – $300.000 |

> **Decisión estratégica:** arrancar con consultorios médicos como vertical principal. Ticket más alto, menor competencia local, y casos de uso ricos (múltiples especialidades, recordatorios críticos).

---

## 4. Modelo de negocio

### 4.1 Planes de suscripción (preliminar)

| Plan | Precio COP/mes | Profesionales | Sucursales | Citas/mes | Funcionalidades clave |
|---|---|---|---|---|---|
| **Starter** | $59.000 | Hasta 2 | 1 | 200 | Agenda, página pública, recordatorios email |
| **Pro** | $129.000 | Hasta 8 | 2 | 1.500 | + WhatsApp, pagos online (Wompi), reportes |
| **Business** | $249.000 | Hasta 20 | 5 | Ilimitadas | + Multi-sucursal avanzado, API, soporte prioritario |
| **Enterprise** | Custom | Ilimitados | Ilimitadas | Ilimitadas | + SLA, integraciones a medida, onboarding |

### 4.2 Comisión transaccional (opcional, plan Pro+)
- 1.5% sobre pagos procesados vía Wompi (encima de la comisión propia de Wompi).
- Aplica solo si el negocio usa el cobro online integrado.

---

## 5. Usuarios y roles

| Rol | Quién es | Qué hace |
|---|---|---|
| **Super Admin (Turnify)** | Tú / equipo Turnify | Gestiona tenants, planes, métricas globales |
| **Tenant Owner** | Dueño del negocio | Configura todo, ve reportes, gestiona suscripción |
| **Tenant Admin** | Recepcionista / gerente | Gestiona agenda, clientes, profesionales (sin facturación) |
| **Staff (Profesional)** | Médico, barbero, esteticista | Ve su agenda, marca disponibilidad, atiende citas |
| **Customer (Cliente final)** | El paciente / cliente | Reserva citas, ve historial, paga |

---

## 6. Arquitectura técnica

### 6.1 Decisión clave: Modular Monolith con Vertical Slices

**¿Por qué no microservicios?**
- Operacionalmente costosos (varios despliegues, service discovery, observabilidad distribuida).
- Resuelven problemas que tienes con equipos de 30+ personas, no con un equipo de 1.
- La latencia de red entre servicios mata la fluidez que pides.

**¿Por qué Modular Monolith?**
- Un solo despliegue, simple de operar.
- Módulos con fronteras estrictas (cada uno con su propio Domain, Application, Infrastructure).
- Comunicación in-process (rapidísima) vía MediatR.
- **Cualquier módulo se puede extraer como microservicio en el futuro sin reescribir.** Ese es el "escalable" que pides.
- Es el patrón recomendado por Microsoft, ThoughtWorks y la mayoría de arquitectos serios para proyectos nuevos.

### 6.2 Estructura de módulos

```
Turnify.Solution/
├── src/
│   ├── Bootstrapper/
│   │   └── Turnify.Api/                      ← Único proyecto desplegable
│   │
│   ├── Modules/
│   │   ├── Tenants/                          ← Gestión de organizaciones y planes
│   │   │   ├── Turnify.Modules.Tenants.Domain
│   │   │   ├── Turnify.Modules.Tenants.Application
│   │   │   ├── Turnify.Modules.Tenants.Infrastructure
│   │   │   └── Turnify.Modules.Tenants.Api
│   │   │
│   │   ├── Identity/                         ← Auth, usuarios, roles
│   │   ├── Catalog/                          ← Servicios, profesionales, sucursales
│   │   ├── Scheduling/                       ← Horarios, disponibilidad, cálculo de slots
│   │   ├── Booking/                          ← Citas, estados, reglas de reserva
│   │   ├── Payments/                         ← Wompi, transacciones, comprobantes
│   │   ├── Notifications/                    ← Email, WhatsApp, SMS
│   │   └── Reporting/                        ← Reportes y analítica
│   │
│   └── Shared/
│       ├── Turnify.Shared.Kernel/            ← Tipos base, abstracciones
│       ├── Turnify.Shared.Infrastructure/    ← EF base, autenticación común
│       └── Turnify.Shared.Contracts/         ← Eventos públicos entre módulos
│
└── tests/
    ├── Unit/
    ├── Integration/
    └── Architecture/                          ← Tests de fronteras de módulos (NetArchTest)
```

### 6.3 Reglas de comunicación entre módulos

| Tipo | Mecanismo | Cuándo usarlo |
|---|---|---|
| **Query síncrona** | MediatR Request | Booking necesita saber si un Staff existe → query a Catalog |
| **Comando síncrono** | MediatR Request | Booking pide a Payments crear una transacción |
| **Evento de dominio** | MediatR Notification | Booking publica `AppointmentConfirmed` → Notifications lo escucha y manda WhatsApp |
| **Acceso directo a tablas de otro módulo** | ❌ PROHIBIDO | Romper esta regla anula toda la arquitectura |

### 6.4 Cuándo y qué extraer como microservicio (futuro)

| Módulo | Cuándo extraer | Por qué |
|---|---|---|
| Notifications | Cuando >50k notificaciones/día | Carga de I/O independiente, escalable horizontalmente |
| Reporting | Cuando los reportes pesados afecten la API principal | Mover a una réplica de lectura |
| Payments | Si se contrata equipo dedicado de finanzas | Compliance y auditoría aislada |

---

## 7. Stack tecnológico definitivo

### 7.1 Backend

| Componente | Tecnología | Justificación |
|---|---|---|
| Runtime | **.NET 8 (LTS)** | Soporte hasta nov 2026, mejor rendimiento de la historia de .NET |
| Framework Web | **ASP.NET Core Web API + Minimal APIs** | Minimal APIs ~20% más rápidas que Controllers para endpoints simples |
| ORM | **Entity Framework Core 8** + `Pomelo.EntityFrameworkCore.MySql` | Estándar de facto MySQL en .NET, mantenido y rápido |
| Base de datos | **MySQL 8.0+** (InnoDB) | Tu requerimiento; sólido, conocido, gran soporte |
| Cache distribuido | **Redis 7** | Sesiones, slots calculados, rate limiting |
| Mediator / CQRS | **MediatR** | Estándar para Modular Monolith |
| Validación | **FluentValidation** | Validación declarativa, testeable |
| Mapping | **Mapster** | Más rápido que AutoMapper, sintaxis limpia |
| Background Jobs | **Hangfire** + Redis | Recordatorios, envío de emails/WhatsApp, reportes |
| Real-time | **SignalR** | Notificación de nuevas citas al admin en vivo |
| Logging | **Serilog** + Seq (dev) / Application Insights (prod) | Logging estructurado |
| Testing | **xUnit + FluentAssertions + Testcontainers** | Tests con MySQL real en containers |
| Migraciones | **EF Core Migrations** | Controladas por código |

### 7.2 Frontend

**Decisión:** **Blazor United (.NET 8)** — un solo proyecto Blazor que decide por página el modo de render. Esto es nuevo en .NET 8 y resuelve el clásico dilema "WASM o Server".

| Tipo de página | Modo de render | Por qué |
|---|---|---|
| Landing pública del negocio (`/{slug}`) | **Static SSR** | SEO, primera carga rapidísima |
| Flujo de reserva del cliente final | **Server interactive** | Validación en tiempo real, sin descargar runtime WASM |
| Panel admin del tenant | **WebAssembly** | App rica, después de auth, vale la pena el download inicial |
| Vista de staff (móvil) | **WebAssembly** + PWA | Funciona offline, instalable |

**Librería UI:** **MudBlazor** (Material Design, gratis, comunidad activa, todos los componentes que necesitas).

### 7.3 DevOps e infraestructura

| Componente | Tecnología | Justificación |
|---|---|---|
| Cloud | **Azure** | Ya lo manejas en el trabajo |
| Hosting API | **Azure App Service** (Linux) | Simple, autoescalado, integración con Azure Monitor |
| MySQL | **Azure Database for MySQL Flexible Server** | Backups automáticos, alta disponibilidad |
| Redis | **Azure Cache for Redis** | Mismo VNet que la API, latencia mínima |
| Storage | **Azure Blob Storage** | Imágenes de perfil, comprobantes de pago |
| CI/CD | **GitHub Actions** | Gratis para repos públicos, suficiente para privados |
| Secretos | **Azure Key Vault** | NUNCA secretos en `appsettings.json` |
| Observabilidad | **Application Insights** | Trazas distribuidas, logs, métricas |
| CDN | **Azure CDN** | Para assets estáticos y Blazor WASM bundle |

### 7.4 Integraciones externas

| Servicio | Para qué | Notas |
|---|---|---|
| **Wompi** | Pagos online (tarjeta, PSE, Nequi, Bancolombia) | Gateway preferido en Colombia |
| **WhatsApp Business Cloud API** | Recordatorios y confirmaciones | Vía Meta directamente o proveedor (Twilio/360dialog) |
| **SendGrid** | Email transaccional | Plan gratis 100/día, escalable |
| **Twilio** (opcional) | SMS de respaldo | Solo si WhatsApp falla |

---

## 8. Diseño de base de datos (MySQL 8)

### 8.1 Convenciones globales

- **Engine:** InnoDB para todas las tablas (transacciones, FK, row locking).
- **Charset:** `utf8mb4` con collation `utf8mb4_0900_ai_ci`.
- **PK:** `BIGINT UNSIGNED AUTO_INCREMENT` para entidades de alto volumen; `CHAR(26)` (ULID) para entidades expuestas en URLs públicas (no exponer IDs incrementales por seguridad).
- **TenantId:** **TODA** tabla de negocio lleva `tenant_id BIGINT UNSIGNED NOT NULL` con FK e índice. EF Core aplicará un *global query filter* para que sea imposible olvidarlo.
- **Auditoría:** todas las tablas llevan `created_at`, `updated_at`, `created_by`, `updated_by`, `is_deleted` (soft delete).
- **Timezone:** todas las fechas se guardan en **UTC** (`DATETIME`). Se convierten a la zona del tenant en la capa de aplicación.

### 8.2 Módulo Tenants

#### `tenants`
| Campo | Tipo | Notas |
|---|---|---|
| id | BIGINT UNSIGNED PK | |
| public_id | CHAR(26) UNIQUE | ULID, expuesto en URLs |
| slug | VARCHAR(60) UNIQUE | Para `turnify.co/clinica-vital` |
| name | VARCHAR(150) | |
| nit | VARCHAR(20) NULL | NIT colombiano |
| timezone | VARCHAR(50) DEFAULT 'America/Bogota' | |
| currency | CHAR(3) DEFAULT 'COP' | |
| status | ENUM('active','suspended','cancelled') | |
| owner_user_id | BIGINT UNSIGNED FK | |
| created_at, updated_at | DATETIME | |

#### `subscription_plans`
| Campo | Tipo | Notas |
|---|---|---|
| id | INT UNSIGNED PK | |
| code | VARCHAR(30) UNIQUE | `starter`, `pro`, `business` |
| name | VARCHAR(60) | |
| price_monthly_cop | DECIMAL(10,2) | |
| max_staff | INT | NULL = ilimitado |
| max_locations | INT | |
| max_appointments_month | INT | |
| features_json | JSON | Flags como `whatsapp`, `online_payments` |

#### `tenant_subscriptions`
| Campo | Tipo |
|---|---|
| id | BIGINT UNSIGNED PK |
| tenant_id | BIGINT UNSIGNED FK |
| plan_id | INT UNSIGNED FK |
| started_at | DATETIME |
| ends_at | DATETIME |
| status | ENUM('trial','active','past_due','cancelled') |
| trial_ends_at | DATETIME NULL |

### 8.3 Módulo Identity

#### `users`
| Campo | Tipo | Notas |
|---|---|---|
| id | BIGINT UNSIGNED PK | |
| public_id | CHAR(26) UNIQUE | |
| tenant_id | BIGINT UNSIGNED FK NULL | NULL = super admin global |
| email | VARCHAR(150) | UNIQUE por tenant |
| email_normalized | VARCHAR(150) | Para búsqueda case-insensitive |
| password_hash | VARCHAR(255) | **Argon2id** (no PBKDF2 por defecto) |
| first_name, last_name | VARCHAR(80) | |
| phone | VARCHAR(20) | |
| email_verified_at | DATETIME NULL | |
| two_factor_enabled | BOOLEAN DEFAULT 0 | |
| two_factor_secret | VARBINARY(255) NULL | Encriptado con Data Protection |
| status | ENUM('active','locked','disabled') | |
| failed_login_attempts | TINYINT UNSIGNED DEFAULT 0 | |
| locked_until | DATETIME NULL | |
| last_login_at | DATETIME NULL | |

**Índice único compuesto:** `(tenant_id, email_normalized)`.

#### `roles`, `user_roles`, `permissions`, `role_permissions`
Estándar RBAC. Roles del sistema (`SuperAdmin`, `TenantOwner`, `TenantAdmin`, `Staff`, `Customer`) más roles custom por tenant.

#### `refresh_tokens`
| Campo | Tipo | Notas |
|---|---|---|
| id | BIGINT UNSIGNED PK | |
| user_id | BIGINT UNSIGNED FK | |
| token_hash | CHAR(64) | SHA-256 del token, nunca el token plano |
| expires_at | DATETIME | |
| revoked_at | DATETIME NULL | |
| replaced_by_id | BIGINT UNSIGNED NULL | Para detectar reuso (rotation) |
| ip_address, user_agent | VARCHAR | Forensia |

#### `login_attempts`
Registro de cada intento de login (exitoso o no) para detección de brute force y auditoría.

### 8.4 Módulo Catalog

#### `locations`
Sucursales del tenant.

| Campo | Tipo |
|---|---|
| id, public_id, tenant_id | |
| name | VARCHAR(150) |
| address, city, department | VARCHAR |
| phone | VARCHAR(20) |
| timezone | VARCHAR(50) |
| latitude, longitude | DECIMAL(10,7) NULL |
| is_active | BOOLEAN |

#### `service_categories`
Para agrupar servicios (ej: "Medicina General", "Especialistas").

#### `services`
| Campo | Tipo | Notas |
|---|---|---|
| id, public_id, tenant_id | | |
| category_id | FK NULL | |
| name | VARCHAR(150) | |
| description | TEXT | |
| duration_minutes | SMALLINT UNSIGNED | Ej: 30, 45, 60 |
| buffer_before_minutes | SMALLINT UNSIGNED DEFAULT 0 | Tiempo de preparación |
| buffer_after_minutes | SMALLINT UNSIGNED DEFAULT 0 | Tiempo de limpieza |
| price | DECIMAL(10,2) | |
| color_hex | CHAR(7) | Para el calendario |
| requires_deposit | BOOLEAN | |
| deposit_amount | DECIMAL(10,2) NULL | |
| is_active | BOOLEAN | |

#### `staff`
Profesionales que prestan servicios.

| Campo | Tipo |
|---|---|
| id, public_id, tenant_id | |
| user_id | FK NULL (puede no tener login) |
| first_name, last_name | VARCHAR |
| email, phone | VARCHAR |
| professional_title | VARCHAR(100) NULL |
| bio | TEXT |
| photo_url | VARCHAR(500) |
| is_bookable | BOOLEAN | Si aparece en la página pública |
| is_active | BOOLEAN |

#### `staff_services` (M:N) y `staff_locations` (M:N)
Qué servicios presta cada profesional y en qué sucursales.

### 8.5 Módulo Scheduling

#### `staff_schedules`
Horario laboral semanal recurrente.

| Campo | Tipo |
|---|---|
| id, tenant_id | |
| staff_id | FK |
| location_id | FK |
| day_of_week | TINYINT (0=Domingo, 6=Sábado) |
| start_time | TIME |
| end_time | TIME |
| valid_from | DATE |
| valid_until | DATE NULL |

#### `staff_time_off`
Vacaciones, permisos, bloqueos puntuales.

| Campo | Tipo |
|---|---|
| id, tenant_id | |
| staff_id | FK |
| start_at | DATETIME (UTC) |
| end_at | DATETIME (UTC) |
| reason | VARCHAR(255) |
| type | ENUM('vacation','sick','personal','other') |

#### `holidays`
Feriados del tenant (puede heredar feriados nacionales colombianos como base).

### 8.6 Módulo Booking

#### `customers`
Clientes finales del negocio. **No son usuarios del sistema** salvo que se registren explícitamente.

| Campo | Tipo |
|---|---|
| id, public_id, tenant_id | |
| user_id | FK NULL (si se registró con login) |
| first_name, last_name | VARCHAR |
| email | VARCHAR |
| phone | VARCHAR |
| document_type | ENUM('CC','CE','TI','PA') NULL |
| document_number | VARCHAR(20) NULL |
| birth_date | DATE NULL |
| notes | TEXT (notas internas del negocio) |
| created_at | DATETIME |

**Índice:** `(tenant_id, phone)` y `(tenant_id, email)` para búsqueda rápida.

#### `appointments` (LA tabla central)

| Campo | Tipo | Notas |
|---|---|---|
| id | BIGINT UNSIGNED PK | |
| public_id | CHAR(26) UNIQUE | Para URLs como `/cita/01HXY...` |
| tenant_id | BIGINT UNSIGNED FK | |
| customer_id | FK | |
| staff_id | FK | |
| service_id | FK | |
| location_id | FK | |
| start_at | DATETIME (UTC) | **INDEX** |
| end_at | DATETIME (UTC) | |
| status | ENUM(...) | Ver máquina de estados abajo |
| price_total | DECIMAL(10,2) | Snapshot del precio al momento de reservar |
| deposit_amount | DECIMAL(10,2) | |
| deposit_paid | BOOLEAN | |
| customer_notes | TEXT | Notas del cliente al reservar |
| internal_notes | TEXT | Notas del negocio |
| source | ENUM('public_page','admin','staff','api') | |
| cancellation_reason | VARCHAR(255) NULL | |
| cancelled_at | DATETIME NULL | |
| cancelled_by_user_id | FK NULL | |
| created_at, updated_at | DATETIME | |
| row_version | TIMESTAMP | Para concurrencia optimista |

**Índices críticos:**
- `(tenant_id, staff_id, start_at, end_at)` → consulta de disponibilidad
- `(tenant_id, customer_id, start_at)` → historial del cliente
- `(tenant_id, status, start_at)` → recordatorios programados

**Estados del Appointment (state machine):**
```
pending_payment ──→ confirmed ──→ in_progress ──→ completed
       │                │                              ▲
       │                ├──→ no_show                   │
       │                ├──→ cancelled_by_customer     │
       │                └──→ cancelled_by_business     │
       └──→ payment_failed → cancelled                 │
                                                       │
                              rescheduled ─────────────┘
```

#### `appointment_status_history`
Cada transición de estado se registra. Inmutable. Para auditoría y reportes.

### 8.7 Módulo Payments

#### `transactions`
| Campo | Tipo |
|---|---|
| id, public_id, tenant_id | |
| appointment_id | FK NULL |
| customer_id | FK |
| amount | DECIMAL(10,2) |
| currency | CHAR(3) |
| type | ENUM('deposit','full_payment','refund','subscription') |
| gateway | ENUM('wompi','manual_nequi','manual_bancolombia','cash') |
| gateway_reference | VARCHAR(100) NULL | ID en Wompi |
| status | ENUM('pending','approved','declined','voided','refunded') |
| receipt_url | VARCHAR(500) NULL | Comprobante manual subido |
| paid_at | DATETIME NULL |
| created_at | DATETIME |

#### `webhook_events` (idempotencia de webhooks de Wompi)
| Campo | Tipo |
|---|---|
| id | BIGINT UNSIGNED PK |
| provider | ENUM('wompi') |
| event_id | VARCHAR(100) UNIQUE | Para evitar procesar 2 veces |
| event_type | VARCHAR(50) |
| payload_json | JSON |
| processed_at | DATETIME NULL |
| processing_error | TEXT NULL |

### 8.8 Módulo Notifications

#### `notification_templates`
Plantillas customizables por tenant (con fallback a plantilla del sistema).

| Campo | Tipo |
|---|---|
| id, tenant_id (NULL = sistema) | |
| code | VARCHAR(60) (`appointment_reminder_24h`, etc.) |
| channel | ENUM('email','whatsapp','sms') |
| subject | VARCHAR(200) NULL (solo email) |
| body | TEXT (con tokens `{{customer.name}}`, `{{appointment.start_at}}`) |
| is_active | BOOLEAN |

#### `notification_log`
| Campo | Tipo |
|---|---|
| id, tenant_id | |
| recipient_type | ENUM('customer','staff','tenant_admin') |
| recipient_id | BIGINT |
| channel | ENUM(...) |
| template_code | VARCHAR(60) |
| status | ENUM('queued','sent','delivered','failed','read') |
| sent_at, delivered_at, read_at | DATETIME NULL |
| provider_message_id | VARCHAR(100) NULL |
| error_message | TEXT NULL |

### 8.9 Auditoría

#### `audit_logs`
| Campo | Tipo |
|---|---|
| id | BIGINT UNSIGNED PK |
| tenant_id | FK NULL |
| user_id | FK NULL |
| entity_type | VARCHAR(60) (`Appointment`, `Customer`, ...) |
| entity_id | BIGINT |
| action | ENUM('create','update','delete','login','logout','payment') |
| old_values | JSON NULL |
| new_values | JSON NULL |
| ip_address | VARCHAR(45) |
| user_agent | VARCHAR(500) |
| created_at | DATETIME |

**Índice:** `(tenant_id, entity_type, entity_id, created_at)`.

---

## 9. Lógica de negocio por módulo

### 9.1 Cálculo de disponibilidad (el algoritmo crítico)

Este es **el** algoritmo más importante del sistema. Se ejecuta cada vez que un cliente abre la página de reservas.

**Input:**
- `tenantId`, `serviceId`, `staffId` (o "cualquiera"), `locationId`, `dateFrom`, `dateTo`

**Pasos:**
1. Obtener `service.duration` y buffers.
2. Obtener `staff_schedules` del/los staff aplicables que se traslapen con el rango.
3. Restar `staff_time_off` y `holidays`.
4. Restar `appointments` ya existentes del staff en estado `confirmed | in_progress | pending_payment`.
5. Generar slots discretos según un *grid* (ej: cada 15 min) que **caben completos** en los huecos.
6. Aplicar reglas del tenant: anticipación mínima (ej: no aceptar reservas con menos de 2h), anticipación máxima (ej: máx 60 días).

**Optimización:**
- El resultado se **cachea en Redis** con clave `availability:{tenantId}:{serviceId}:{staffId}:{date}` y TTL de 60 segundos.
- Se invalida cuando se crea/cancela/modifica una cita del staff en esa fecha (publicación de evento de dominio).
- Para tenants con muchos staff: paralelizar el cálculo por staff con `Parallel.ForEachAsync`.

### 9.2 Reserva de cita (caso crítico de concurrencia)

**Riesgo:** dos clientes intentan reservar el mismo slot al mismo tiempo.

**Solución:**
1. Validar disponibilidad (cache).
2. Iniciar transacción MySQL con `SERIALIZABLE` o usar `SELECT ... FOR UPDATE` sobre las citas del staff en el rango.
3. Re-verificar que no haya solapamiento (constraint a nivel de aplicación + chequeo final).
4. Insertar el `appointment` con status `pending_payment` (si requiere depósito) o `confirmed`.
5. Publicar evento de dominio `AppointmentCreated` → invalida caché de disponibilidad, dispara notificaciones.
6. Commit.

**Validación adicional:** unique constraint compuesto a nivel de DB sobre `(staff_id, start_at)` previene duplicados exactos como última línea de defensa.

### 9.3 Recordatorios automáticos

**Job recurrente (Hangfire) cada 5 minutos:**
1. Buscar citas en estado `confirmed` cuya `start_at` esté entre 24h±5min y que no tengan recordatorio enviado.
2. Encolar job de envío de notificación según preferencia del tenant (WhatsApp si plan lo permite, email si no).
3. Marcar en `notification_log`.

**Mismo flujo para recordatorio de 1h antes** (configurable por tenant).

### 9.4 Cancelación con políticas

Cada tenant define su política:
- "Cancelación gratis hasta X horas antes"
- "Cancelación con menos de X horas: pierde el depósito"
- "No-show: pierde el depósito + flag interno"

Esto se implementa como reglas en el módulo Booking que se aplican al cambiar el estado.

### 9.5 Multi-sucursal

Un staff puede trabajar en varias sucursales con horarios distintos. La disponibilidad se calcula por `(staff, location)`. La página pública del negocio puede ser una sola con selector de sucursal o subdominio por sucursal en planes superiores.

---

## 10. Estrategia de seguridad (no negociable)

### 10.1 Autenticación

| Aspecto | Implementación |
|---|---|
| Hashing de contraseñas | **Argon2id** (NIST recomendado, mejor que el PBKDF2 default de Identity) — librería: `Konscious.Security.Cryptography` |
| Tokens | JWT firmados con **RS256** (clave asimétrica, rotable), payload mínimo |
| Vida del access token | **15 minutos** |
| Refresh tokens | Hasheados (SHA-256) en DB, rotación obligatoria, detección de reuso → revoca toda la cadena |
| 2FA | TOTP (apps tipo Authy/Google Authenticator) opcional, **obligatorio para TenantOwner** en plan Business+ |
| Verificación de email | Obligatoria antes de operar |
| Lockout | 5 intentos fallidos → bloqueo 15 min, escalado exponencial |

### 10.2 Autorización

- **RBAC + Policy-Based Authorization** de ASP.NET Core.
- Cada endpoint declara la policy que requiere (`[Authorize(Policy = "CanManageAppointments")]`).
- **Tenant isolation:** middleware que extrae `tenantId` del JWT y lo inyecta en `ICurrentTenant`. EF Core aplica `HasQueryFilter(e => e.TenantId == _currentTenant.Id)` globalmente.

### 10.3 Protección de datos

| Vector | Defensa |
|---|---|
| SQL Injection | EF Core parametrizado, **prohibido** `FromSqlRaw` con concatenación |
| XSS | Blazor escapa por defecto; CSP estricta en headers |
| CSRF | Antiforgery tokens en formularios server-rendered; SameSite=Strict en cookies |
| IDOR | Global query filter por TenantId + uso de `public_id` (ULID) en URLs, nunca IDs incrementales |
| Mass assignment | DTOs explícitos (no bindear entidades directamente) |
| Datos sensibles en reposo | **Data Protection API** para 2FA secrets, tokens de webhooks |
| TLS | HTTPS obligatorio, HSTS con preload, TLS 1.2+ |
| Headers de seguridad | CSP, X-Content-Type-Options, X-Frame-Options=DENY, Referrer-Policy |

### 10.4 Rate limiting (built-in en .NET 8)

| Endpoint | Límite |
|---|---|
| `/auth/login` | 5 req/min por IP |
| `/auth/register` | 3 req/hora por IP |
| `/public/*/availability` | 60 req/min por IP |
| API general (autenticada) | 600 req/min por usuario |

### 10.5 Webhooks

- Validación de firma HMAC en webhooks de Wompi.
- Idempotencia: tabla `webhook_events` con `event_id` único.
- Procesamiento asíncrono vía Hangfire para no bloquear respuesta a Wompi.

### 10.6 Auditoría y cumplimiento

- Toda acción sensible se registra en `audit_logs`.
- Habeas Data (Ley 1581 Colombia): endpoints para que un cliente exporte sus datos y solicite borrado.
- Política de retención: logs >2 años se archivan a Blob Storage (compresión).

### 10.7 Secretos

- **Cero secretos en el repo** (incluido `appsettings.Development.json` con keys reales).
- En desarrollo: `dotnet user-secrets`.
- En producción: **Azure Key Vault** + Managed Identity.
- Rotación de keys de JWT cada 90 días con dual-signing durante el cambio.

---

## 11. Estrategia de rendimiento (la fluidez que pides)

### 11.1 Caché por capas

| Capa | Tecnología | Qué cachea | TTL |
|---|---|---|---|
| Browser | Cache-Control headers | Assets estáticos | 1 año (con hash en filename) |
| CDN | Azure CDN | Bundle Blazor WASM, imágenes | Largo |
| App | `IMemoryCache` | Configuración del tenant, plantillas | 5 min |
| Distribuido | Redis | Sesiones, slots de disponibilidad, rate limits | 60s – 5min |
| DB | Query plan cache | — | Automático MySQL |

### 11.2 Optimizaciones EF Core

- `AsNoTracking()` por defecto en todas las queries de lectura.
- **Compiled queries** (`EF.CompileAsyncQuery`) en hot paths (búsqueda de disponibilidad).
- Proyecciones a DTOs con `Select` directo, nunca cargar entidades completas para mostrar.
- `Split queries` para evitar el problema cartesiano en includes complejos.
- Conexión con `Pooling=true;MinPoolSize=10;MaxPoolSize=200`.

### 11.3 Optimizaciones MySQL

- **Índices compuestos** alineados con queries reales (cubrir el `WHERE` y el `ORDER BY`).
- `EXPLAIN` obligatorio en queries críticas durante desarrollo.
- Particionado de `audit_logs` y `notification_log` por mes cuando crezcan.
- Configuración: `innodb_buffer_pool_size` 60–70% RAM, `innodb_flush_log_at_trx_commit=1` (durabilidad).
- Slow query log en producción con threshold 500ms.

### 11.4 Comunicación

- **Response compression** (Brotli + Gzip) en ASP.NET Core.
- **HTTP/2** habilitado en Azure App Service.
- **JSON serializer:** `System.Text.Json` con source generators (sin reflection en runtime).
- Pagination obligatoria en listados (cursor-based para feeds, offset para tablas admin).

### 11.5 Background work

- Cualquier operación >200ms que el usuario no necesita esperar va a Hangfire (envío de emails, generación de reportes, integraciones externas).
- Polly para retries con backoff exponencial en llamadas a Wompi/WhatsApp.

### 11.6 Frontend

- **Blazor United** decide modo por página (no descargas WASM en la landing pública).
- Lazy loading de assemblies WASM por área del admin.
- Pre-rendering en server para primer paint instantáneo.
- Compresión Brotli del bundle WASM (~70% reducción).

### 11.7 Métricas objetivo (SLO)

| Métrica | Objetivo |
|---|---|
| p95 latencia API | < 200ms |
| p99 latencia API | < 500ms |
| Time to First Byte página pública | < 300ms |
| Disponibilidad mensual | 99.5% (Pro), 99.9% (Business) |

---

## 12. Flujos clave (end-to-end)

### Flujo 1: Cliente reserva una cita

1. Cliente entra a `turnify.co/clinica-vital` → SSR estático, primer paint <300ms.
2. Selecciona servicio "Consulta General".
3. Selecciona profesional (o "cualquiera disponible").
4. Frontend pide `/api/public/availability?...` → backend consulta Redis primero, calcula si miss.
5. Cliente elige slot, completa datos (nombre, teléfono, email, documento).
6. Si servicio requiere depósito → flujo Wompi:
   - Backend crea `appointment` con status `pending_payment`.
   - Crea `transaction` y obtiene `payment_link` de Wompi.
   - Redirige al cliente al checkout de Wompi.
   - Wompi confirma vía webhook → handler valida firma, marca transacción `approved`, cita pasa a `confirmed`.
7. Si no requiere depósito → cita se crea directamente como `confirmed`.
8. Eventos de dominio disparados:
   - `AppointmentConfirmed` → Notifications encola WhatsApp/email de confirmación.
   - SignalR notifica al admin del tenant en vivo.
   - Cache de disponibilidad invalidado.

### Flujo 2: Recordatorio automático

1. Hangfire cron job cada 5 min ejecuta `SendUpcomingAppointmentReminders`.
2. Query de citas confirmadas en ventana 24h±5min sin recordatorio enviado.
3. Por cada cita: encolar job de envío vía canal preferente del tenant.
4. Worker procesa: llama API WhatsApp/SendGrid → registra en `notification_log`.
5. Si falla: Polly reintenta 3 veces → si persiste, alerta al admin.

### Flujo 3: Cancelación por el cliente

1. Cliente abre link único `/cita/{public_id}` (token firmado).
2. Ve detalles, presiona "Cancelar".
3. Backend valida política de cancelación del tenant.
4. Si dentro de plazo → cita pasa a `cancelled_by_customer`, depósito se reembolsa (Wompi refund API).
5. Si fuera de plazo → cita se cancela pero depósito se retiene.
6. `appointment_status_history` registra el cambio.
7. Notifica al staff.

---

## 13. Plan de implementación por fases

### Fase 0 — Setup (Semana 1)
- Repo en GitHub con estructura del modular monolith.
- Solution con todos los proyectos vacíos.
- Pipeline GitHub Actions: build + test.
- Compose local: API + MySQL + Redis + Seq.
- Plantillas de issues y PRs.

### Fase 1 — Core Identity + Tenants (Semanas 2-3)
- Registro de tenant + owner.
- Login con JWT + refresh.
- Verificación email.
- Estructura multitenant con global query filters.
- Tests de integración con Testcontainers.

**Entregable:** un tenant puede registrarse, hacer login, ver un dashboard vacío.

### Fase 2 — Catalog (Semana 4)
- CRUD de sucursales, servicios, profesionales, horarios.
- UI admin en Blazor.

**Entregable:** un tenant puede configurar todo su catálogo.

### Fase 3 — Página pública + Booking básico (Semanas 5-6)
- Landing del tenant en SSR.
- Algoritmo de disponibilidad.
- Flujo de reserva sin pago online.
- Listado de citas en admin.

**Entregable:** un cliente final puede reservar una cita gratis, el negocio la ve.

### Fase 4 — Notifications (Semana 7)
- Email transaccional con SendGrid.
- Recordatorios con Hangfire.
- Confirmaciones automáticas.

**Entregable:** todo el flujo de booking con notificaciones por email.

### Fase 5 — Payments con Wompi (Semanas 8-9)
- Integración Wompi (checkout + webhooks).
- Manejo de depósitos.
- Reembolsos.

**Entregable:** se puede cobrar online por una cita.

### Fase 6 — WhatsApp + Real-time (Semana 10)
- Integración WhatsApp Business Cloud API.
- SignalR para notificaciones en vivo al admin.

**Entregable:** producto vendible. **MVP cerrado.**

### Fase 7 — Hardening + Lanzamiento (Semanas 11-12)
- Auditoría de seguridad (OWASP ZAP).
- Pruebas de carga (k6).
- Monitoreo (Application Insights + alertas).
- Onboarding con primer cliente real.

---

## 14. Próximos pasos concretos

1. **Validar este documento** — leer completo, decirme qué cambias o profundizas.
2. **Definir naming oficial** — ¿"Turnify" te gusta o prefieres otro? Esto afecta dominios, repo, etc.
3. **Crear el repo y la estructura base** — yo te guío con los `dotnet new` exactos.
4. **Diseñar el primer módulo (Tenants)** — vamos paso a paso, no todo a la vez.
5. **Crear las primeras migraciones** — generar el schema MySQL inicial.

---

## 15. Decisiones que dejé tomadas (y puedes objetar)

| Decisión | Alternativa que rechacé | Razón |
|---|---|---|
| Modular Monolith | Microservicios | Operacionalmente prematuro |
| Blazor United | React + ASP.NET API separados | Stack 100% C#, menos contexto switching |
| MySQL | PostgreSQL | Tu requerimiento explícito |
| MediatR + CQRS lite | Servicios tradicionales | Permite extraer módulos a futuro |
| ULID en URLs | GUID o IDs incrementales | Ordenable, opaco, URL-friendly |
| Argon2id | PBKDF2 default de Identity | Más resistente a ataques GPU/ASIC |
| Hangfire | Servicio worker custom | Battle-tested, dashboard incluido |
| Mapster | AutoMapper | Más rápido, menos magia |
| MudBlazor | Telerik / Syncfusion | Gratis, MIT, comunidad activa |

---

**Documento listo para revisión. Después de aprobarlo, arrancamos con código.**