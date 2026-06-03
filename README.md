# SLFMS — Smart Logistics & Fleet Management System

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![C#](https://img.shields.io/badge/C%23-13-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![Architecture](https://img.shields.io/badge/Architecture-DDD%20%7C%20CQRS%20%7C%20Vertical%20Slice-success)
![Tests](https://img.shields.io/badge/Tests-xUnit%20%7C%20NetArchTest-informational)

SLFMS is a large-scale backend system built with .NET 9 to explore and apply modern enterprise software architecture patterns in a realistic logistics domain.

The system models real-world logistics operations: fleet management, driver lifecycle, shift scheduling, shipment tracking with full route history, warehouse and inventory management, billing, payment processing, insurance claims, and a complete identity layer.

The primary goal is to demonstrate practical, production-quality implementation of:

- Domain-Driven Design (DDD) with rich Aggregate Roots
- Vertical Slice Architecture (features own their full stack)
- CQRS with MediatR
- Rich Domain Models (behavior-first, not anemic)
- Domain Events dispatched via EF Core `SaveChangesInterceptor`
- Business Rules as first-class objects
- Result Pattern for explicit failure modeling
- Soft Delete & Audit Logging as cross-cutting infrastructure concerns
- ASP.NET Core Identity with JWT, Refresh Tokens, and TOTP 2FA
- Architecture Tests with NetArchTest

---

## Domain Model

The system is organized around 10 Aggregate Roots, each owning its own consistency boundary.

| Aggregate Root | Responsibility | Key Value Objects |
|----------------|---------------|-------------------|
| `Vehicle` | Fleet lifecycle, maintenance, fuel tracking | `VehiclePlateNumber`, `VehicleSpecification`, `FuelConsumption` |
| `Driver` | Driver lifecycle, license, rating, shift assignment | `DriverLicense`, `DriverRating`, `PhoneNumber` |
| `Shift` | Shift creation, start/complete/cancel lifecycle | `ShiftId` |
| `Shipment` | Full shipment lifecycle, route history, packages | `TrackingInfo`, `DeliveryAddress`, `Weight`, `Dimensions`, `RoutePoint` |
| `Warehouse` | Storage locations, item assignment, capacity | `Address`, `Capacity`, `StorageLocationId` |
| `InventoryItem` | Stock tracking, reorder thresholds, product info | `StockLevel`, `ProductInfo`, `Weight` |
| `User` | Identity, login/logout, account management | *(via ASP.NET Core Identity + custom domain)* |
| `Invoice` | Invoice creation, payment, cancellation | `InvoiceItem` |
| `Payment` | Payment processing, refunds, failure tracking | `TransactionInfo`, `PaymentMethod` |
| `InsuranceClaim` | Claim submission, review, approval/rejection | `ClaimAmount`, `ClaimDocument`, `ClaimNumber` |

---

## Architecture

The project combines Domain-Driven Design, Vertical Slice Architecture, and CQRS — all grounded in Clean Architecture layering.

```
┌─────────────────────────────────────┐
│               API Layer             │
│  Controllers · Middleware · Auth    │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│          Application Layer          │
│  Features (Vertical Slices)         │
│  Commands · Queries · Handlers      │
│  Validators · DTOs · Mappings       │
│  Pipeline Behaviors                 │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│             Domain Layer            │
│  Aggregates · Entities              │
│  Value Objects · Domain Events      │
│  Business Rules · SharedKernel      │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│         Infrastructure Layer        │
│  EF Core · SQL Server               │
│  SaveChanges Interceptors           │
│  Repositories · Identity            │
│  Email · Token Blacklist            │
└─────────────────────────────────────┘
```

---

## Key Architectural Decisions

### Vertical Slice Architecture

Each Aggregate Root owns its full Application layer slice:

```
Application/Features/
├── Vehicles/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   └── Mappings/
├── Drivers/
├── Shifts/
├── Shipments/
├── Warehouses/
├── Inventory/
├── Invoices/
├── Payments/
├── Claim/
└── Users/
```

This keeps business capabilities cohesive and avoids horizontal coupling between unrelated features.

---

### Rich Domain Model

Business behavior lives inside the Aggregate, not in service classes.

```csharp
// Driver
var result = Driver.Hire(name, license);
driver.Suspend(DriverSuspensionReason.Misconduct);
driver.Reactivate();
driver.AssignShift(shiftId);
driver.RecordTripRating(4.5);

// Shipment
var shipment = Shipment.Create(senderId, address, trackingNumber);
shipment.AddPackage(description, weight, dimensions);
shipment.AssignCarrier("FedEx");
shipment.Dispatch();
shipment.AddRoutePoint(location, description, arrivedAt, RoutePointType.OutForDelivery);
shipment.MarkDelivered(deliveredAt, receivedBy);

// Vehicle
vehicle.ScheduleMaintenance(date, description);
vehicle.RecordFuelConsumption(consumption);
vehicle.Retire();
```

State transitions are never done by directly assigning properties. Every state change goes through a domain method that enforces the relevant business rules.

---

### Business Rules as First-Class Objects

Business invariants are encapsulated in dedicated rule classes.

```csharp
public sealed class DriverMustNotBeSuspendedRule : IBusinessRule
{
    private readonly DriverStatus _status;

    public DriverMustNotBeSuspendedRule(DriverStatus status) => _status = status;

    public bool IsBroken() => _status == DriverStatus.Suspended;

    public Error Error => DriverErrors.AlreadySuspended;
}
```

Aggregates check rules before executing any operation:

```csharp
public Result Suspend(DriverSuspensionReason reason)
{
    CheckRule(new DriverMustNotBeSuspendedRule(Status));
    // ...
}
```

The system contains **26 business rules** across all aggregates.

---

### Domain Events

Aggregates raise Domain Events on every meaningful state change. There are **76 Domain Events** across the system.

```csharp
// Examples
VehicleRegisteredEvent
VehicleRetiredEvent
MaintenanceScheduledEvent
FuelConsumptionRecordedEvent

DriverHiredEvent
DriverSuspendedEvent
DriverReactivatedEvent
DriverPerformanceDroppedEvent
DriverShiftAssignedEvent

ShipmentCreatedEvent
ShipmentDispatchedEvent
ShipmentDeliveredEvent
ShipmentDeliveryFailedEvent
ShipmentCancelledEvent
RoutePointAddedEvent
ShipmentOutForDeliveryEvent

InvoiceCreatedEvent
InvoicePaidEvent
PaymentProcessedEvent
PaymentRefundedEvent

ClaimSubmittedEvent
UserRegisteredDomainEvent
UserTwoFactorEnabledDomainEvent
// ... and more
```

Domain Events are dispatched automatically through an EF Core `SaveChangesInterceptor`:

```csharp
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        // Collect all pending events from tracked aggregates
        var domainEvents = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // Publish via MediatR before committing
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);

        // Clear to prevent re-publishing
        entities.ForEach(e => e.ClearDomainEvents());

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

---

### Result Pattern

Expected failures are modeled explicitly — no exceptions for business rule violations.

```csharp
// Result<T> for operations that return a value
Result<Driver> result = Driver.Hire(name, license);

if (result.IsFailure)
    return result.Error; // typed Error, not an exception

var driver = result.Value;

// Result for void operations
Result suspendResult = driver.Suspend(reason);
```

---

### Soft Delete & Audit Logging

Implemented as cross-cutting infrastructure concerns via EF Core interceptors. Aggregates implement the relevant interfaces:

```csharp
public class Vehicle : AggregateRoot<VehicleId>, IAudiatable, ISoftDeletable
{
    // Soft delete
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    // Audit
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
}
```

The `SoftDeleteInterceptor` handles deletion automatically — business logic never touches deletion flags directly.

---

### CQRS

Commands mutate state. Queries read state. Never mixed.

**Commands (examples)**

```
HireDriverCommand
SuspendDriverCommand
CreateShipmentCommand
DispatchShipmentCommand
AssignCarrierCommand
ScheduleMaintenanceCommand
SubmitClaimCommand
ProcessPaymentCommand
```

**Queries (examples)**

```
GetDriverByIdQuery
GetShipmentByIdQuery
GetVehicleByIdQuery
GetWarehouseByIdQuery
GetProfileQuery
GetClaimsByStatusQuery
```

---

### MediatR Pipeline Behaviors

Every request passes through a shared pipeline before reaching its handler.

```
Request
  ↓
LoggingBehavior      (structured request/response logging)
  ↓
ValidationBehavior   (FluentValidation — fails fast before handler)
  ↓
Handler
```

---

## Features

### Fleet Management
- Vehicle registration with uniqueness enforcement (plate number)
- Fuel consumption recording
- Maintenance scheduling with date and description validation
- Vehicle status tracking (Available → InMaintenance → Retired)
- Soft delete with audit trail

### Driver Management
- Driver onboarding with license validation
- Suspension and reactivation with reason tracking
- Performance-based rating (triggers `DriverPerformanceDroppedEvent` below 3.5)
- Shift assignment with overlap prevention
- Name and license update

### Shift Management
- Shift creation with duration validation
- Lifecycle: Planned → Active → Completed / Cancelled

### Shipment Management
- Shipment creation with priority levels (Economy, Standard, Express, Overnight)
- Package management (weight, dimensions, fragile/refrigeration flags, declared value)
- Carrier assignment
- Dispatch with pre-conditions (must have packages + carrier)
- Route point recording (Transit, CustomsClearance, SortingFacility, OutForDelivery)
- Status auto-update on route events (e.g. `OutForDelivery` route point → `ShipmentOutForDeliveryEvent`)
- Delivery confirmation (with receiver name)
- Delivery failure recording
- Cancellation with reason

### Warehouse Management
- Warehouse registration with address
- Storage location management with capacity
- Item assignment/unassignment to locations

### Inventory Management
- Inventory item creation with product info and stock level
- Stock additions and removals
- Reorder threshold management
- Item deactivation
- Out-of-stock and reorder-needed events

### Insurance Claims
- Claim submission against a shipment (with optional supporting document)
- Claim item line management
- Lifecycle: Submitted → UnderReview → Approved / Rejected
- Payment processing on approved claims

### Financial Operations
- Invoice creation and management
- Payment processing via a mock payment gateway (injectable for real gateways)
- Payment refunds and failure tracking

### Identity & Access Management
- User registration and login
- Email confirmation workflow
- Password reset workflow
- JWT access tokens + sliding Refresh Tokens
- Two-Factor Authentication (TOTP via Otp.NET)
- Token blacklisting for logout revocation
- Role-Based and Claims-Based Authorization
- Account lock/unlock, deactivation, reactivation

---

## Security

### Authentication Stack
- ASP.NET Core Identity (persistence + password hashing)
- JWT access tokens
- Refresh tokens (sliding expiry)
- Two-Factor Authentication — TOTP via `Otp.NET`, compatible with standard authenticator apps

### Token Revocation
Logged-out tokens are tracked in an in-memory blacklist service until their natural expiry. The `BlacklistMiddleware` rejects blacklisted tokens on every request.

### Authorization
- Role-Based Authorization
- Claims-Based Authorization
- Admin management endpoints separated into a dedicated `AdminController`

---

## Tech Stack

| Category | Technology |
|----------|------------|
| Runtime | .NET 9 |
| Language | C# 13 |
| API | ASP.NET Core |
| ORM | Entity Framework Core 9 |
| Database | SQL Server |
| Architecture | DDD + CQRS + Vertical Slice |
| Mediator | MediatR 14 |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Authentication | JWT + ASP.NET Core Identity |
| Two-Factor Auth | Otp.NET (TOTP) |
| Email | MailKit |
| Testing | xUnit + FluentAssertions |
| Architecture Testing | NetArchTest |

---

## Project Structure

```
SLFMS/
├── API/
│   ├── Controllers/          # AdminController, AuthenticationController,
│   │                         # ShipmentController, VehicleController, ...
│   └── Middleware/           # BlacklistMiddleware
│
├── Application/
│   ├── Features/
│   │   ├── Vehicles/         # Commands, Queries, DTOs, Mappings
│   │   ├── Drivers/
│   │   ├── Shifts/
│   │   ├── Shipments/
│   │   ├── Warehouses/
│   │   ├── Inventory/
│   │   ├── Invoices/
│   │   ├── Payments/
│   │   ├── Claim/
│   │   └── Users/
│   └── Common/
│       ├── Behaviors/        # LoggingBehavior, ValidationBehavior
│       └── Interfaces/       # IEmailService, ITokenService, ICacheService, ...
│
├── Domain/
│   ├── SharedKernel/         # AggregateRoot<T>, Entity<T>, ValueObject,
│   │                         # IBusinessRule, Result<T>, Error, DomainEvent
│   ├── Vehicles/
│   ├── Drivers/
│   ├── Shifts/
│   ├── Shipments/
│   ├── Warehouse/
│   ├── Inventory/
│   ├── Invoices/
│   ├── Payments/
│   ├── Claims/
│   └── Users/
│
├── Infrastructure/
│   ├── Presistence/
│   │   ├── Configurations/   # EF Core fluent configs per aggregate
│   │   └── Interceptors/     # DomainEventInterceptor, SoftDeleteInterceptor
│   ├── Repositories/         # One repository per aggregate root
│   ├── Services/             # EmailService, TokenBlacklistService,
│   │                         # InMemoryCacheService, MockPaymentGateway
│   └── Identity/             # ApplicationUser, IdentityService
│
└── Domain.Tests/
    ├── DriverTests.cs
    ├── ShipmentTests.cs
    ├── VehicleTests.cs
    ├── ShiftTests.cs
    ├── WarehouseTests.cs
    ├── InventoryItemTests.cs
    └── InvoiceTests.cs
```

---

## Running the Project

### Clone

```bash
git clone https://github.com/Mohammed-Atef2004/Smart-Logistics-Fleet-Management-System.git
```

### Configure

Update `appsettings.Development.json` with your:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=SLFMS;..."
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "...",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "Host": "...",
    "Port": 587,
    "Username": "...",
    "Password": "..."
  }
}
```

### Apply Migrations

```bash
dotnet ef database update \
  --project Infrastructure \
  --startup-project API
```

### Run

```bash
dotnet run --project API
```

Swagger UI: `https://localhost:{port}/swagger`

---

## Testing

```bash
dotnet test
```

The test suite covers:

- **Domain Unit Tests** — aggregate behavior for all 10 aggregates (hire/suspend/reactivate, dispatch/deliver/cancel, add-stock/remove-stock, etc.)
- **Architecture Tests** — layer dependency rules enforced with NetArchTest (Domain has no Infrastructure references, etc.)

---

## Current Status

### Implemented
- All 10 domain aggregates with rich behavior
- 76 domain events + automatic dispatch via EF Core interceptor
- 26 business rules
- 32 value objects
- CQRS with 10 vertical slices
- Full authentication stack (JWT + Refresh Tokens + TOTP 2FA + token blacklisting)
- Soft delete and audit logging via interceptors
- Domain unit tests + architecture tests

### Planned
- Docker support
- GitHub Actions CI/CD
- Redis caching (replace in-memory cache)
- Outbox Pattern (reliable event delivery)
- Background processing
- OpenTelemetry + structured logging
- Integration tests

---

## License

MIT License
