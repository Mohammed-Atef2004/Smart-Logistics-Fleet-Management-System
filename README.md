# 🚚 SLFMS — Shipping & Logistics Fleet Management System

A fully integrated system for managing transportation and shipping companies, built with **Clean Architecture** and **.NET**.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Current Progress](#current-progress)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Modules](#modules)
- [Roles & Permissions](#roles--permissions)
- [Getting Started](#getting-started)
- [Tech Stack](#tech-stack)

---

## Overview

SLFMS is an enterprise-grade fleet and logistics management platform covering:

- 🚛 **Fleet Management** — Vehicles, drivers, and maintenance
- 📦 **Shipment Tracking** — End-to-end shipment lifecycle
- 🏭 **Warehouse & Inventory** — Stock management and alerts
- 💰 **Billing & Payments** — Invoices, payments, and insurance claims
- 👥 **Users & Auth** — Role-based access control with JWT
- 🔔 **Notifications** — Email, SMS, and push notifications
- 📊 **Analytics** — Performance reports and dashboards

---

## ✅ Current Progress

| Layer / Component | Status |
|---|---|
| Solution & project structure | ✅ Done |
| Clean Architecture setup | ✅ Done |
| MediatR pipeline & behaviors | ✅ Done |
| Vehicle aggregate (Domain) | ✅ Done |
| Driver aggregate (Domain) | ✅ Done |
| Base pipeline (Validation, Logging, Transaction) | ✅ Done |
| Shipment module | 🔜 Next |
| Warehouse module | 🔜 Next |
| Billing module | 🔜 Next |
| Users & Auth | 🔜 Next |
| API Controllers | 🔜 Next |
| Tests | 🔜 Next |

---

## Architecture

The project follows **Clean Architecture** with strict dependency rules:

```
API → Application → Domain ← Infrastructure
```

- **Domain** — Core business logic, entities, value objects, domain events. Zero external dependencies.
- **Application** — Use cases (Commands/Queries via CQRS), DTOs, validators, pipeline behaviors.
- **Infrastructure** — EF Core, repositories, JWT, email/SMS, background jobs, messaging.
- **API** — Controllers, middleware, Swagger, request/response contracts.

---

## Project Structure

```
SLFMS.sln
├── src/
│   ├── SLFMS.Domain          # Entities, value objects, domain events, interfaces
│   ├── SLFMS.Application     # CQRS handlers, validators, DTOs, behaviors
│   ├── SLFMS.Infrastructure  # EF Core, services, repositories, messaging
│   ├── SLFMS.API             # Controllers, middleware, Swagger
│   └── SLFMS.Shared          # Constants, extensions, helpers
└── tests/
    ├── SLFMS.UnitTests
    ├── SLFMS.IntegrationTests
    └── SLFMS.ArchitectureTests
```

---

## Modules

### 1. 🚛 Fleet Management
Manages vehicles, drivers, and maintenance records.

- Business rules: vehicle must be available before assignment, driver max 8 hours/day, maintenance every 10,000 km.
- Entities: `Vehicle` (AR), `Driver`, `MaintenanceRecord`
- Domain Events: `VehicleCreated`, `VehicleStatusChanged`, `DriverAssigned`, `MaintenanceScheduled`, `MaintenanceCompleted`

### 2. 📦 Shipment Tracking
Manages shipments from creation to delivery.

- Business rules: unique tracking number, at least one package per shipment, delivered shipments cannot be modified.
- Entities: `Shipment` (AR), `Package`, `Route`, `TrackingUpdate`

### 3. 🏭 Warehouse & Inventory
Manages warehouses, stock levels, and storage locations.

- Business rules: quantity cannot go negative, alert triggered on low stock.
- Entities: `Warehouse` (AR), `InventoryItem` (AR), `StorageLocation`

### 4. 💰 Billing & Payments
Handles invoices, payments, and insurance claims.

- Business rules: paid invoices cannot be modified, payment amount must match invoice total.
- Entities: `Invoice` (AR), `Payment`, `InsuranceClaim`

### 5. 👥 Users & Auth
Role-based authentication and authorization.

- Business rules: unique email per user, password minimum 8 characters.
- Entities: `User` (AR), `Role` (AR), `UserRole`

### 6. 🔔 Notifications
Sends Email, SMS, and Push notifications.

- Entities: `Notification` (AR), `Alert`

### 7. 📊 Analytics
Provides dashboards and performance reports for drivers, revenue, and shipments.

---

## Roles & Permissions

| Role | Access |
|---|---|
| **Admin** | Full access to all modules |
| **Driver** | Manage own shipments only |
| **WarehouseManager** | Manage inventory and warehouses |
| **Customer** | Create and track own shipments |
| **Accountant** | Invoices and payments |

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (or PostgreSQL)
- Docker (optional)

### Run the API

```bash
git clone https://github.com/your-org/SLFMS.git
cd SLFMS

# Restore dependencies
dotnet restore

# Apply migrations
dotnet ef database update --project src/SLFMS.Infrastructure --startup-project src/SLFMS.API

# Run
dotnet run --project src/SLFMS.API
```

Swagger UI available at: `https://localhost:5001/swagger`

---

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core |
| CQRS | MediatR |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Auth | JWT Bearer |
| Password Hashing | BCrypt |
| Background Jobs | Hangfire |
| Messaging | RabbitMQ |
| Testing | xUnit + Moq + Testcontainers |
| Architecture Tests | NetArchTest |

---

## 📁 Key Design Decisions

- **Aggregate Roots** enforce invariants and raise domain events internally.
- **Outbox Pattern** ensures reliable event publishing without distributed transactions.
- **Pipeline Behaviors** handle cross-cutting concerns: logging, validation, transactions, and performance monitoring.
- **Value Objects** (`Money`, `Address`, `Email`, `PhoneNumber`, `GeoCoordinate`, `Weight`) encapsulate domain concepts with immutability.
- **Soft Delete** is applied across all entities via a global query filter.

---

> Built with ❤️ using Clean Architecture & Domain-Driven Design principles.
