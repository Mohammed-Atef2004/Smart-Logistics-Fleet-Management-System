# SLFMS — Smart Logistics & Fleet Management System
<div align="center">
  
![.NET 9](https://img.shields.io/badge/.NET-9-512BD4) ![C#](https://img.shields.io/badge/C%23-13-239120) ![EF Core](https://img.shields.io/badge/EF_Core-9-512BD4) ![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927) ![MediatR](https://img.shields.io/badge/MediatR-14-blue)

</div>

<div align="center">
  
A production-grade logistics platform built with **Vertical Slice Architecture**, **Domain-Driven Design**, and **CQRS**.

</div>

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Bounded Contexts & Aggregates](#bounded-contexts--aggregates)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
- [Domain Model Highlights](#domain-model-highlights)
- [Design Decisions](#design-decisions)
- [Implementation Status](#implementation-status)

---

## Overview

SLFMS is a backend system for managing fleet operations, shipments, warehouse inventory, billing, and identity access — all within a single deployable .NET 9 API.

The system is built around **Vertical Slice Architecture** where each Aggregate Root owns its full slice from the HTTP endpoint down to the database configuration. There are no shared service layers or bloated generic repositories — only focused, cohesive feature slices.

---

## Architecture

```
┌────────────────────────────────────────────────────────┐
│                        API Layer                       │
│     Controllers · Middleware · DI · JWT Auth           │
└─────────────────────┬──────────────────────────────────┘
                      │ MediatR
┌─────────────────────▼──────────────────────────────────┐
│                  Application Layer                     │
│     Commands · Queries · Validators · Event Handlers   │
│         Pipeline Behaviors (Logging · Validation)      │
└─────────────────────┬──────────────────────────────────┘
                      │ Interfaces
┌─────────────────────▼──────────────────────────────────┐
│                   Domain Layer                         │
│  Aggregate Roots · Entities · Value Objects · Events   │
│          Business Rules · Domain Errors                │
└─────────────────────┬──────────────────────────────────┘
                      │ EF Core
┌─────────────────────▼──────────────────────────────────┐
│               Infrastructure Layer                     │
│  Repositories · DbContext · Configurations · Migrations│
│       Interceptors · DomainEventInterceptor            │
│   Identity (ASP.NET Core) · Email (MailKit) · Audit    │
└────────────────────────────────────────────────────────┘
```

### Core Principles

| Principle | Implementation |
|---|---|
| Vertical Slice per Aggregate | Each AR owns its Commands, Queries, DTOs, Repository, DbSet, and EF Configuration |
| Rich Domain Model | Aggregates expose behavior methods, not just properties |
| Task-Based API | No generic CRUD — every endpoint reflects a business intent |
| CQRS | Commands and Queries are fully separated via MediatR |
| Domain Events | Cross-aggregate communication via `DomainEventInterceptor` on `SaveChangesAsync` |
| Repository only for ARs | Inner Entities (e.g. `Package`, `MaintenanceSchedule`) are never accessed directly |
| Result Pattern | No exceptions for expected failures — `Result<T>` flows from Domain to API |
| Token Blacklist | Revoked access tokens are tracked in-memory until expiry via `BlacklistMiddleware` |

---

## Bounded Contexts & Aggregates

### Fleet Management

| Aggregate Root | Inner Entities | Value Objects |
|---|---|---|
| `Vehicle` | `MaintenanceSchedule` | `VehiclePlateNumber`, `VehicleSpecification`, `FuelConsumption` |
| `Driver` | — | `DriverId`, `DriverLicense`, `DriverRating` |
| `Shift` | — | `ShiftId` |

### Shipment Management

| Aggregate Root | Inner Entities | Value Objects |
|---|---|---|
| `Shipment` | `Package` | `ShipmentId`, `DeliveryAddress`, `TrackingInfo`, `RoutePoint`, `Weight`, `Dimensions` |

> Route is modeled as `List<RoutePoint>` (Value Objects) — not a `Route` entity.

### Warehouse & Inventory

| Aggregate Root | Inner Entities | Value Objects |
|---|---|---|
| `Warehouse` | `StorageLocation` | `WarehouseId`, `Address`, `Capacity` |
| `InventoryItem` | — | `InventoryItemId`, `ProductInfo`, `StockLevel`, `Weight` |

### Identity & Access

| Aggregate Root | Inner Entities | Value Objects |
|---|---|---|
| `User` | — | `Email`, `PhoneNumber`, `FullName`, `Username` |

> User authentication is backed by **ASP.NET Core Identity** (`IdentityUser`). The Domain `User` aggregate holds business state while Identity handles password hashing, email confirmation, and refresh tokens.

---

## Project Structure

```
SLFMS/
├── API/
│   ├── Controllers/
│   │   ├── AdminController.cs
│   │   ├── ApiController.cs              ← Base controller (HandleFailure)
│   │   ├── AuthenticationController.cs
│   │   ├── DriverController.cs
│   │   ├── InventoryController.cs
│   │   ├── ProfileController.cs
│   │   ├── SecurityController.cs         ← 2FA management
│   │   ├── ShiftController.cs
│   │   ├── ShipmentController.cs
│   │   ├── VehicleController.cs
│   │   └── WarehousesController.cs
│   ├── Middleware/
│   │   ├── BlacklistMiddleware.cs         ← JWT token revocation
│   │   └── ExceptionHandlingMiddleware.cs
│   └── Program.cs
│
├── Application/
│   ├── Common/
│   │   └── Behaviors/
│   │       ├── LoggingBehaviour.cs
│   │       └── ValidationBehaviour.cs
│   └── Features/
│       ├── Vehicle/
│       │   ├── Commands/
│       │   │   ├── RegisterNewVehicle/
│       │   │   ├── ScheduleMaintenance/
│       │   │   ├── RecordFuelConsumption/
│       │   │   ├── UpdateVehicleStatus/
│       │   │   └── RetireVehicle/
│       │   └── Queries/ GetById/
│       ├── Driver/
│       │   ├── Commands/
│       │   │   ├── HireDriver/
│       │   │   ├── Suspend/
│       │   │   ├── Reactivate/
│       │   │   ├── AssignShift/
│       │   │   ├── RecordRating/
│       │   │   ├── UpdateName/
│       │   │   └── UpdateLicence/
│       │   └── Queries/ GetById/ GetAll/
│       ├── Shift/
│       │   ├── Commands/ Create/ StartShift/ CancelShift/ CompleteShift/
│       │   └── Queries/ GetById/ GetAll/
│       ├── Shipment/
│       │   ├── Commands/
│       │   │   ├── Create/
│       │   │   ├── AddPackage/ RemovePackage/
│       │   │   ├── AddRoutePoint/
│       │   │   ├── AssignCarrier/
│       │   │   ├── Dispatch/
│       │   │   ├── MarkDelivered/ MarkDeliveryFailed/
│       │   │   ├── Cancel/
│       │   │   └── UpdateDeliveryAddress/
│       │   └── Queries/ GetById/ GetAll/ GetPackages/
│       ├── Inventory/
│       │   ├── Commands/
│       │   │   ├── CreateInventoryItem/
│       │   │   ├── AdjustStock/
│       │   │   ├── DeactivateItem/
│       │   │   └── UpdateWeight/
│       │   └── Queries/ GetById/ GetAll/
│       ├── Warehouse/
│       │   ├── Commands/
│       │   │   ├── CreateWarehouse/
│       │   │   ├── AddStorageLocation/ RemoveStorageLocation/
│       │   │   ├── AssignItemToLocation/ UnassignItemFromLocation/
│       │   │   ├── DeactivateWarehouse/
│       │   │   └── UpdateAddress/
│       │   └── Queries/ GetById/ GetAll/
│       └── Users/
│           ├── Commands/
│           │   ├── Authentication/
│           │   │   ├── Register/
│           │   │   ├── Login/            ← supports 2FA flow
│           │   │   ├── Logout/           ← token blacklisting
│           │   │   ├── RefreshToken/
│           │   │   ├── ConfirmEmail/
│           │   │   ├── ForgotPassword/
│           │   │   ├── ResetPassword/
│           │   │   └── VerifyTwoFactor/
│           │   ├── Profile/ UpdateName/ UpdatePhone/
│           │   └── Security/ EnableTwoFactor/ DisableTwoFactor/
│           └── Queries/ GetProfile/
│
├── Domain/
│   ├── SharedKernel/
│   │   ├── AggregateRoot.cs
│   │   ├── Entity.cs
│   │   ├── ValueObject.cs
│   │   ├── DomainEvent.cs
│   │   ├── Result.cs / GenericResult.cs
│   │   ├── Error.cs
│   │   └── IBusinessRule.cs
│   ├── Vehicles/
│   │   ├── Vehicle.cs                   ← Aggregate Root
│   │   ├── MaintenanceSchedule.cs       ← Entity inside AR
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Rules/
│   │   ├── Errors/
│   │   └── IVehicleRepository.cs
│   ├── Drivers/
│   ├── Shifts/
│   ├── Shipments/
│   ├── Warehouse/
│   ├── Inventory/
│   ├── Users/
│   │   ├── User.cs                      ← Aggregate Root
│   │   ├── ValueObjects/                ← Email, Username, FullName, PhoneNumber
│   │   ├── Errors/
│   │   └── IUserRepository.cs
│   ├── Interfaces/
│   │   ├── Repositories/  IUnitOfWork, IUserRepository, ...
│   │   └── Services/      ITokenService, IEmailService, IIdentityService,
│   │                      ITotpService, IAuditService, ITokenBlacklistService
│   ├── Settings/           ApiSettings, JwtSettings, EmailSettings
│   └── DomainServices/
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Data/ AppDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── VehicleConfiguration.cs
│   │   │   ├── DriverConfiguration.cs
│   │   │   ├── ShipmentConfiguration.cs
│   │   │   ├── ShiftConfiguration.cs
│   │   │   ├── WarehouseConfiguration.cs
│   │   │   ├── InventoryItemConfiguration.cs
│   │   │   └── UserConfiguration.cs
│   │   ├── Interceptors/
│   │   │   └── DomainEventInterceptor.cs
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── VehicleRepository.cs
│   │   ├── DriverRepository.cs
│   │   ├── ShiftRepository.cs
│   │   ├── ShipmentRepository.cs
│   │   ├── WarehouseRepository.cs
│   │   ├── InventoryItemRepository.cs
│   │   ├── UserRepository.cs
│   │   └── Shared/ GenericRepository.cs / UnitOfWork.cs
│   └── Services/
│       ├── TokenService.cs              ← JWT access + refresh tokens
│       ├── IdentityService.cs           ← ASP.NET Core Identity wrapper
│       ├── EmailService.cs              ← MailKit / SMTP
│       ├── TotpService.cs               ← Otp.NET (TOTP 2FA)
│       ├── AuditService.cs
│       └── TokenBlacklistService.cs     ← In-memory token revocation
│
└── Domain.Tests/
    ├── VehicleTests.cs
    ├── DriverTests.cs
    ├── ShipmentTests.cs
    └── ShiftTests.cs
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9 / C# 13 |
| API | ASP.NET Core (MVC Controllers) |
| CQRS / Mediator | MediatR 14 |
| Validation | FluentValidation |
| ORM | Entity Framework Core 9 |
| Database | SQL Server 2022 |
| Identity | ASP.NET Core Identity + EF Core |
| Authentication | JWT Bearer (access + refresh tokens) |
| Two-Factor Auth | TOTP via Otp.NET |
| Password Hashing | BCrypt.Net-Next |
| Email | MailKit (SMTP) |
| Mapping | AutoMapper |
| Architecture Tests | NetArchTest.Rules |
| Testing | xUnit |
| Domain Events | EF Core `SaveChangesInterceptor` |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)
- SMTP server (or MailHog for local dev)

### Setup

```bash
# 1. Clone the repository
git clone https://github.com/Mohammed-Atef2004/Smart-Logistics-Fleet-Management-System.git
cd Smart-Logistics-Fleet-Management-System

# 2. Configure settings in API/appsettings.Development.json:
# "ConnectionStrings": { "DefaultConnection": "Server=.;Database=SLFMS;Trusted_Connection=True;" }
# "JwtSettings": { "SecretKey": "...", "Issuer": "...", "Audience": "...", "ExpiryMinutes": 15 }
# "EmailSettings": { "SmtpHost": "...", "SmtpPort": 587, "Username": "...", "Password": "..." }
# "ApiSettings": { "BaseUrl": "https://localhost:7xxx" }

# 3. Apply migrations
dotnet ef database update --project Infrastructure --startup-project API

# 4. Run
dotnet run --project API
```

The API will be available at `https://localhost:7xxx` with Swagger at `/swagger`.

### Running Tests

```bash
dotnet test Domain.Tests
```

---

## API Reference

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/authentication/register` | Register a new user (sends confirmation email) |
| `POST` | `/api/authentication/login` | Login — returns JWT or triggers 2FA |
| `POST` | `/api/authentication/logout` | Logout (blacklists current token) |
| `POST` | `/api/authentication/refresh-token` | Exchange refresh token for new access token |
| `GET` | `/api/authentication/confirm-email` | Confirm email via link |
| `POST` | `/api/authentication/forgot-password` | Request password reset email |
| `POST` | `/api/authentication/reset-password` | Reset password via token |
| `POST` | `/api/authentication/verify-2fa` | Verify TOTP code after login |

### Profile

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/profile` | Get current user profile |
| `PUT` | `/api/profile/name` | Update display name |
| `PUT` | `/api/profile/phone` | Update phone number |

### Security (2FA)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/security/enable-2fa` | Enable two-factor authentication |
| `POST` | `/api/security/disable-2fa` | Disable two-factor authentication |

### Vehicles

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/vehicles` | Register a new vehicle |
| `GET` | `/api/vehicles/{id}` | Get vehicle details |
| `PUT` | `/api/vehicles/{id}/status` | Update vehicle status |
| `POST` | `/api/vehicles/{id}/maintenance` | Schedule maintenance |
| `POST` | `/api/vehicles/{id}/fuel` | Record fuel consumption |
| `DELETE` | `/api/vehicles/{id}` | Retire vehicle |

### Drivers

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/drivers` | Hire a driver |
| `GET` | `/api/drivers/{id}` | Get driver details |
| `GET` | `/api/drivers` | Get all drivers |
| `PUT` | `/api/drivers/{id}/suspend` | Suspend driver |
| `PUT` | `/api/drivers/{id}/reactivate` | Reactivate driver |
| `PUT` | `/api/drivers/{id}/assign-shift` | Assign shift to driver |
| `PUT` | `/api/drivers/{id}/rating` | Record driver rating |

### Shifts

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/shifts` | Create a shift |
| `GET` | `/api/shifts/{id}` | Get shift by ID |
| `GET` | `/api/shifts` | Get all shifts |
| `POST` | `/api/shifts/{id}/start` | Start shift |
| `POST` | `/api/shifts/{id}/complete` | Complete shift |
| `POST` | `/api/shifts/{id}/cancel` | Cancel shift |

### Shipments

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/shipments` | Create shipment |
| `GET` | `/api/shipments/{id}` | Get shipment details |
| `GET` | `/api/shipments` | List all shipments |
| `GET` | `/api/shipments/{id}/packages` | Get shipment packages |
| `POST` | `/api/shipments/{id}/packages` | Add package |
| `DELETE` | `/api/shipments/{id}/packages/{packageId}` | Remove package |
| `POST` | `/api/shipments/{id}/route-points` | Add route point |
| `POST` | `/api/shipments/{id}/assign-carrier` | Assign carrier |
| `POST` | `/api/shipments/{id}/dispatch` | Dispatch shipment |
| `POST` | `/api/shipments/{id}/deliver` | Mark as delivered |
| `POST` | `/api/shipments/{id}/delivery-failed` | Mark delivery failed |
| `POST` | `/api/shipments/{id}/cancel` | Cancel shipment |
| `PATCH` | `/api/shipments/{id}/update-address` | Update delivery address |

### Warehouses

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/warehouses` | Create a warehouse |
| `GET` | `/api/warehouses` | Get all warehouses (`?activeOnly=true`) |
| `GET` | `/api/warehouses/{id}` | Get warehouse by ID |
| `POST` | `/api/warehouses/{warehouseId}/storage-locations` | Add storage location |
| `DELETE` | `/api/warehouses/{warehouseId}/storage-locations/{locationId}` | Remove storage location |
| `POST` | `/api/warehouses/{warehouseId}/storage-locations/{locationId}/items/{itemId}` | Assign item to location |
| `DELETE` | `/api/warehouses/{warehouseId}/storage-locations/{locationId}/items/{itemId}` | Unassign item from location |
| `PUT` | `/api/warehouses/{warehouseId}/address` | Update warehouse address |
| `PUT` | `/api/warehouses/{warehouseId}/deactivate` | Deactivate warehouse |

### Inventory

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/inventory` | Create inventory item |
| `GET` | `/api/inventory` | Get all items (`?activeOnly=true`) |
| `GET` | `/api/inventory/{id}` | Get item by ID |
| `PUT` | `/api/inventory/{id}/stock` | Adjust stock quantity |
| `PUT` | `/api/inventory/{id}/weight` | Update item weight |
| `PUT` | `/api/inventory/{id}/deactivate` | Deactivate item |

---

## Domain Model Highlights

### Result Pattern

All domain operations return `Result` or `Result<T>` — never throw for expected failures:

```csharp
public static Result<Driver> Hire(string name, DriverLicense license)
{
    if (string.IsNullOrWhiteSpace(name))
        return Result<Driver>.Failure(DriverErrors.EmptyName);

    var driver = new Driver(new DriverId(Guid.NewGuid()), name, license);
    driver.AddDomainEvent(new DriverHiredEvent(driver.Id, name));

    return Result<Driver>.Success(driver);
}
```

### Business Rules

Rules are first-class objects implementing `IBusinessRule`:

```csharp
public class DriverMustNotBeSuspendedRule : IBusinessRule
{
    private readonly DriverStatus _status;
    public DriverMustNotBeSuspendedRule(DriverStatus status) => _status = status;

    public bool IsBroken() => _status == DriverStatus.Suspended;
    public Error Error => DriverErrors.AlreadySuspended;
}
```

### Domain Events via EF Interceptor

Domain events are dispatched automatically inside `SaveChangesAsync` via `DomainEventInterceptor` — no manual publish calls needed:

```csharp
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        var entities = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }
}
```

### MediatR Pipeline

```
Request
  → LoggingBehavior      (logs request name + payload)
  → ValidationBehavior   (runs FluentValidation validators)
  → Handler              (executes business logic)
```

### Authentication Flow

```
Register → Email Confirmation → Login
  ↓ (if 2FA enabled)
  → Returns UserId + RequiresTwoFactor: true
  → POST /verify-2fa with TOTP code
  → Returns JWT access token + refresh token

Logout → Token JTI added to in-memory blacklist
       → BlacklistMiddleware rejects further requests with that token
```

---

## Design Decisions

**Why Vertical Slices per Aggregate instead of per Bounded Context?**
Each Aggregate Root is the true unit of consistency. Grouping by Bounded Context would co-locate unrelated aggregates (e.g. `Vehicle` and `Driver`) and encourage shortcuts that violate aggregate boundaries.

**Why SaveChangesInterceptor for Domain Events instead of outbox?**
For the current scale, in-process dispatch before commit is sufficient and keeps the infrastructure simple. An outbox pattern (`OutboxMessage`) can be layered in later without touching domain logic.

**Why OwnsMany/OwnsOne for inner Entities instead of separate DbSets?**
`Package`, `MaintenanceSchedule`, and `StorageLocation` are part of their parent aggregate's consistency boundary. Giving them independent `DbSet`s would leak their existence to the outside world and invite direct access bypassing the aggregate root.

**Why no generic `IRepository<T>`?**
Generic repositories promote the illusion of uniformity across aggregates that have fundamentally different query and persistence needs. Each repository interface is defined in the Domain and implemented with only the methods that aggregate actually needs.

**Why in-memory token blacklist instead of Redis?**
For the current scale, an in-memory `ConcurrentDictionary` keyed by JTI and cleaned up on expiry is sufficient. A distributed cache (Redis) can be swapped in transparently via the `ITokenBlacklistService` interface without touching any other layer.

**Why ASP.NET Core Identity alongside the Domain User aggregate?**
Identity handles the security-sensitive concerns (password hashing, lockout policy, email confirmation tokens) that are well-solved problems. The Domain `User` aggregate holds business state (rating, status, audit trail) that Identity doesn't model.

---

## Implementation Status

| Aggregate | Domain | Application | Infrastructure | Tests |
|---|---|---|---|---|
| Vehicle | ✅ | ✅ | ✅ | ✅ |
| Driver | ✅ | ✅ | ✅ | ✅ |
| Shift | ✅ | ✅ | ✅ | ✅ |
| Shipment | ✅ | ✅ | ✅ | ✅ |
| Warehouse | ✅ | ✅ | ✅ | ✅ |
| InventoryItem | ✅ | ✅ | ✅ | ✅ |
| User | ✅ | ✅ | ✅ | — |
| Invoice | ✅ | ✅ | ✅ | ✅ |
| Payment | 📋 | — | — | — |
| InsuranceClaim | 📋 | — | — | — |

✅ Done &nbsp;·&nbsp; 📋 Planned

---

## License

MIT
