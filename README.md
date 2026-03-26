# SLFMS — Smart Logistics & Fleet Management System

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![MediatR](https://img.shields.io/badge/MediatR-12.x-512BD4?style=for-the-badge)

**A production-grade logistics platform built with Vertical Slice Architecture, Domain-Driven Design, and CQRS.**

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

---

## Overview

SLFMS is a backend system for managing fleet operations, shipments, warehouse inventory, billing, and identity access — all within a single deployable .NET 9 API.

The system is built around **Vertical Slice Architecture** where each **Aggregate Root** owns its full slice from the HTTP endpoint down to the database configuration. There are no shared service layers or bloated generic repositories — only focused, cohesive feature slices.

---

## Architecture

```
┌────────────────────────────────────────────────────────┐
│                        API Layer                       │
│              Controllers · Middleware · DI             │
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
└────────────────────────────────────────────────────────┘
```

### Core Principles

| Principle | Implementation |
|---|---|
| **Vertical Slice per Aggregate** | Each AR owns its Commands, Queries, DTOs, Repository, DbSet, and EF Configuration |
| **Rich Domain Model** | Aggregates expose behavior methods, not just properties |
| **Task-Based API** | No generic CRUD — every endpoint reflects a business intent |
| **CQRS** | Commands and Queries are fully separated via MediatR |
| **Domain Events** | Cross-aggregate communication via `DomainEventInterceptor` on `SaveChangesAsync` |
| **Repository only for ARs** | Inner Entities (e.g. `Package`, `MaintenanceSchedule`) are never accessed directly |
| **Result Pattern** | No exceptions for expected failures — `Result<T>` flows from Domain to API |

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
| `InventoryItem` | — | `InventoryItemId`, `ProductInfo`, `StockLevel` |

### Identity & Access

| Aggregate Root | Inner Entities | Value Objects |
|---|---|---|
| `User` | — | `Email`, `PhoneNumber`, `FullName`, `Username` |

---

## Project Structure

```
SLFMS/
├── API/
│   ├── Controllers/
│   │   ├── VehicleController.cs
│   │   ├── DriverController.cs
│   │   ├── ShipmentController.cs
│   │   └── ShiftController.cs
│   ├── Middleware/
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
│       │   └── Queries/GetById/
│       ├── Driver/
│       │   ├── Commands/
│       │   │   ├── HireDriver/
│       │   │   ├── Suspend/
│       │   │   ├── Reactivate/
│       │   │   ├── AssignShift/
│       │   │   ├── RecordRating/
│       │   │   ├── UpdateName/
│       │   │   └── UpdateLicence/
│       │   └── Queries/GetById/ GetAll/
│       ├── Shift/
│       │   ├── Commands/ Create/ StartShift/ CancelShift/ CompleteShift/
│       │   └── Queries/ GetById/ GetAll/
│       └── Shipment/
│           ├── Commands/
│           │   ├── Create/
│           │   ├── AddPackage/ RemovePackage/
│           │   ├── AddRoutePoint/
│           │   ├── AssignCarrier/
│           │   ├── Dispatch/
│           │   ├── MarkDelivered/ MarkDeliveryFailed/
│           │   ├── Cancel/
│           │   └── UpdateDeliveryAddress/
│           └── Queries/ GetById/ GetAll/ GetPackages/
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
│   │   ├── Vehicle.cs                  ← Aggregate Root
│   │   ├── MaintenanceSchedule.cs      ← Entity inside AR
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
│   └── DomainServices/
│
├── Infrastructure/
│   ├── Presistence/
│   │   ├── Data/AppDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── VehicleConfiguration.cs
│   │   │   ├── DriverConfiguration.cs
│   │   │   ├── ShipmentConfiguration.cs
│   │   │   ├── ShiftConfiguration.cs
│   │   │   ├── WarehouseConfiguration.cs
│   │   │   └── InventoryItemConfiguration.cs
│   │   ├── Interceptors/
│   │   │   └── DomainEventInterceptor.cs
│   │   └── Migrations/
│   └── Repositories/
│       ├── Vehicle/VehicleRepository.cs
│       ├── DriverRepository.cs
│       ├── ShiftRepository.cs
│       ├── ShipmentRepository.cs
│       ├── WarehouseRepository.cs
│       ├── InventoryItemRepository.cs
│       └── Shared/ GenericRepository.cs / UnitOfWork.cs
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
| CQRS / Mediator | MediatR 12 |
| Validation | FluentValidation |
| ORM | Entity Framework Core 9 |
| Database | SQL Server 2022 |
| Mapping | AutoMapper |
| Testing | xUnit |
| Domain Events | EF Core `SaveChangesInterceptor` |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)

### Setup

```bash
# 1. Clone the repository
git clone https://github.com/your-org/slfms.git
cd slfms

# 2. Set the connection string
# In API/appsettings.Development.json:
# "ConnectionStrings": { "DefaultConnection": "Server=.;Database=SLFMS;..." }

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
| `PUT` | `/api/shifts/{id}/start` | Start shift |
| `PUT` | `/api/shifts/{id}/complete` | Complete shift |
| `PUT` | `/api/shifts/{id}/cancel` | Cancel shift |

### Shipments

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/shipments` | Create shipment |
| `GET` | `/api/shipments/{id}` | Get shipment details |
| `GET` | `/api/shipments` | List all shipments |
| `POST` | `/api/shipments/{id}/packages` | Add package |
| `DELETE` | `/api/shipments/{id}/packages/{pkgId}` | Remove package |
| `POST` | `/api/shipments/{id}/route` | Add route point |
| `PUT` | `/api/shipments/{id}/assign-carrier` | Assign carrier |
| `PUT` | `/api/shipments/{id}/dispatch` | Dispatch shipment |
| `PUT` | `/api/shipments/{id}/delivered` | Mark as delivered |
| `PUT` | `/api/shipments/{id}/failed` | Mark delivery failed |
| `PUT` | `/api/shipments/{id}/cancel` | Cancel shipment |
| `PUT` | `/api/shipments/{id}/address` | Update delivery address |

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

        // Dispatch events before saving
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

---

## Design Decisions

**Why Vertical Slices per Aggregate instead of per Bounded Context?**  
Each Aggregate Root is the true unit of consistency. Grouping by Bounded Context would co-locate unrelated aggregates (e.g. `Vehicle` and `Driver`) and encourage shortcuts that violate aggregate boundaries.

**Why `SaveChangesInterceptor` for Domain Events instead of outbox?**  
For the current scale, in-process dispatch before commit is sufficient and keeps the infrastructure simple. An outbox pattern (`OutboxMessage`) can be layered in later without touching domain logic.

**Why `OwnsMany`/`OwnsOne` for inner Entities instead of separate DbSets?**  
`Package`, `MaintenanceSchedule`, and `StorageLocation` are part of their parent aggregate's consistency boundary. Giving them independent DbSets would leak their existence to the outside world and invite direct access bypassing the aggregate root.

**Why no generic `IRepository<T>`?**  
Generic repositories promote the illusion of uniformity across aggregates that have fundamentally different query and persistence needs. Each repository interface is defined in the Domain and implemented with only the methods that aggregate actually needs.

---

## Current Implementation Status

| Aggregate | Domain | Application | Infrastructure | Tests |
|---|:---:|:---:|:---:|:---:|
| Vehicle | ✅ | ✅ | ✅ | ✅ |
| Driver | ✅ | ✅ | ✅ | ✅ |
| Shift | ✅ | ✅ | ✅ | ✅ |
| Shipment | ✅ | ✅ | ✅ | ✅ |
| Warehouse | ✅ | ✅ | ✅ | — |
| InventoryItem | ✅ | ✅ | ✅ | — |
| User | ✅ | — | — | — |
| Invoice | 📋 | — | — | — |
| Payment | 📋 | — | — | — |
| InsuranceClaim | 📋 | — | — | — |

> ✅ Done · 🔄 In Progress · 📋 Planned

---

## License

MIT
