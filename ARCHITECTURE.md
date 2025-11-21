# ComplianceBot - Architecture Documentation

## Table of Contents

1. [Overview](#overview)
2. [Architectural Patterns](#architectural-patterns)
3. [Multi-Tenancy Strategy](#multi-tenancy-strategy)
4. [Layer Responsibilities](#layer-responsibilities)
5. [Data Flow](#data-flow)
6. [Security Architecture](#security-architecture)
7. [Scalability & Performance](#scalability--performance)

---

## Overview

ComplianceBot follows a **Modular Monolith** architecture with **Clean Architecture** principles and **Domain-Driven Design (DDD)**.

### Key Architectural Decisions

- **Modular Monolith:** Single deployable unit, but logically separated into modules
- **Clean Architecture:** Dependencies flow inward (Domain ← Application ← Infrastructure)
- **CQRS:** Separate read and write operations
- **Schema-based Multi-tenancy:** Each tenant has isolated database schema
- **Event-Driven:** Domain events for cross-module communication

---

## Architectural Patterns

### 1. Clean Architecture

```
┌─────────────────────────────────────┐
│        Presentation (API)           │
│  Controllers, Middleware, DTOs      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│         Application Layer           │
│  Commands, Queries, Handlers        │
│  Validators, Mappings, Interfaces   │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│        Infrastructure Layer         │
│  DbContext, Repositories            │
│  External Services, File Storage    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│          Domain Layer               │
│  Entities, Value Objects, Enums     │
│  Domain Events, Business Logic      │
└─────────────────────────────────────┘
```

**Dependency Rule:** Outer layers depend on inner layers, never the reverse.

### 2. CQRS (Command Query Responsibility Segregation)

**Commands (Write):**
- Mutate state
- Return void or simple result
- Validated with FluentValidation
- Example: `CreateReportCommand`

**Queries (Read):**
- Read-only operations
- Return DTOs
- Can use Dapper for performance
- Example: `GetReportQuery`

**Benefits:**
- Clear separation of concerns
- Optimized read/write models
- Easier testing

### 3. Domain-Driven Design

**Bounded Contexts:**
- Identity & Tenants
- Reports
- Documents
- Notifications
- Billing

**Entities:**
- Tenant, User, Report, ReportDocument

**Value Objects:**
- Email, PhoneNumber, Address (future)

**Domain Events:**
- ReportSubmitted, TenantCreated, etc.

---

## Multi-Tenancy Strategy

### Schema-Based Isolation

Each tenant has a **dedicated database schema** for complete data isolation.

**Database Structure:**
```
ComplianceBot_Main
├── [dbo]
│   ├── Tenants (shared)
│   └── SubscriptionPlans (shared)
├── [tenant_abc123]
│   ├── Users
│   ├── Reports
│   ├── ReportDocuments
│   └── AuditLogs
├── [tenant_def456]
│   ├── Users
│   ├── Reports
│   └── ...
```

**How It Works:**

1. **Tenant Middleware** extracts `tenant_id` from JWT token
2. **TenantContext** stores current tenant for the request
3. **DbContext** uses tenant's schema when configuring entities
4. **Global Query Filters** provide additional security (defense in depth)

**Code Example:**
```csharp
// In DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var schema = $"tenant_{_tenantContext.TenantId:N}";
    modelBuilder.Entity<Report>().ToTable("Reports", schema);
}
```

**Benefits:**
- Complete data isolation
- Easy tenant backup/restore
- Clear compliance boundaries
- Simplified queries (no TenantId filters in business logic)

**Tradeoffs:**
- More complex migrations
- Higher database resource usage
- Schema creation overhead

---

## Layer Responsibilities

### Domain Layer

**Purpose:** Core business logic and entities

**Contains:**
- Entities (Tenant, User, Report, etc.)
- Value Objects
- Enums (ReportType, ReportStatus, etc.)
- Domain Exceptions
- Domain Events

**Dependencies:** None (pure C#)

**Rules:**
- No dependencies on other layers
- No infrastructure concerns (database, HTTP, etc.)
- Rich domain models with business logic

### Application Layer

**Purpose:** Application use cases and orchestration

**Contains:**
- Commands and Queries (CQRS)
- Command/Query Handlers
- DTOs
- Validators (FluentValidation)
- Interfaces (IApplicationDbContext, ITenantContext, etc.)
- MediatR Behaviors (validation, logging, performance)

**Dependencies:** Domain

**Rules:**
- Orchestrates domain objects
- Defines interfaces, implementation in Infrastructure
- No direct database or external service access

### Infrastructure Layer

**Purpose:** External concerns and technical implementation

**Contains:**
- DbContext (Entity Framework Core)
- Entity Configurations
- Repositories
- External Service Clients (Azure Blob, etc.)
- Identity & Authentication
- File Storage

**Dependencies:** Domain, Application

**Rules:**
- Implements Application interfaces
- Handles all database access
- Manages external integrations

### API Layer

**Purpose:** HTTP interface and presentation

**Contains:**
- Controllers
- Middleware (Tenant, Exception Handling)
- API Models (if different from DTOs)
- Filters & Attributes

**Dependencies:** Application, Infrastructure

**Rules:**
- Thin controllers (delegate to MediatR)
- No business logic
- HTTP-specific concerns only

---

## Data Flow

### Example: Creating a Report

```
1. Client sends POST /api/v1/reports
   ↓
2. TenantMiddleware extracts tenant_id from JWT
   ↓
3. ReportsController receives request
   ↓
4. CreateReportCommand sent to MediatR
   ↓
5. ValidationBehavior validates command
   ↓
6. CreateReportCommandHandler processes:
   - Gets TenantId from TenantContext
   - Creates Report entity
   - Saves to DbContext (tenant schema)
   ↓
7. DbContext:
   - Sets CreatedAt, CreatedBy (audit)
   - Ensures TenantId is set
   - Saves to tenant_xxx.Reports table
   ↓
8. Handler returns ReportDto
   ↓
9. Controller returns 201 Created
```

### Read Flow (Query)

```
1. Client sends GET /api/v1/reports/{id}
   ↓
2. GetReportQuery sent to MediatR
   ↓
3. GetReportQueryHandler:
   - Queries DbContext (tenant schema)
   - Global query filter ensures TenantId match
   - Projects to DTO with AutoMapper
   ↓
4. Returns ReportDto
```

---

## Security Architecture

### Authentication

- **JWT Bearer Tokens**
- **Claims:**
  - `sub`: User ID
  - `email`: User email
  - `tenant_id`: Tenant ID
  - `role`: User role
  - `permissions`: Array of permissions

### Authorization

**Role-Based:**
- TenantAdmin
- ComplianceOfficer
- ComplianceAnalyst
- Viewer
- SystemAdmin

**Permission-Based:**
- `reports:create`
- `reports:submit`
- `reports:delete`
- `users:manage`

**Implementation:**
```csharp
[Authorize(Policy = "CanSubmitReports")]
public async Task<IActionResult> SubmitReport(...)
```

### Tenant Isolation

**Three Layers of Protection:**

1. **Schema Isolation:** Separate schemas per tenant
2. **Middleware:** Validates tenant from JWT
3. **Query Filters:** EF Core global filters ensure TenantId match

```csharp
// Global query filter
modelBuilder.Entity<Report>()
    .HasQueryFilter(r => r.TenantId == _tenantContext.TenantId);
```

### Audit Logging

All changes tracked:
- Who (CreatedBy, ModifiedBy)
- When (CreatedAt, ModifiedAt)
- What (entity changes)
- Retention: 10 years minimum

---

## Scalability & Performance

### Horizontal Scaling

- **Stateless API:** No in-memory sessions
- **Azure App Service:** Auto-scaling (2-10 instances)
- **Load Balancer:** Azure Application Gateway

### Database Optimization

- **Indexes:** Created on frequently queried columns
- **Read Replicas:** For reporting/analytics
- **Dapper:** Used for complex read queries
- **Connection Pooling:** Enabled

### Caching Strategy

**Redis Cache:**
- Validation rules (24h TTL)
- User sessions
- Temporary wizard data

**Response Caching:**
- Static data (report types, countries)
- ETag support

### Background Processing

**Hangfire:**
- Report generation (async)
- File processing
- Notifications
- Billing

**Azure Service Bus:**
- Cross-module communication
- Event-driven workflows
- Guaranteed delivery

### Performance Targets

| Metric | Target |
|--------|--------|
| API Response (p95) | < 200ms |
| Report Generation | < 2 min |
| File Upload (50MB) | < 30s |
| Concurrent Users | 1,000+ |

---

## Future Considerations

### Microservices Migration Path

If the platform grows significantly:

1. **Extract Modules:** RBE, CEDR, etc. become separate services
2. **Shared Kernel:** Common domain logic
3. **API Gateway:** Ocelot or Azure API Management
4. **Service Discovery:** Consul or Azure Service Fabric

### Event Sourcing

For audit-heavy domains:
- Store all state changes as events
- Rebuild state from event log
- Complete audit trail

### Read Models

For complex reporting:
- Separate read database (SQL or NoSQL)
- Updated via domain events
- Optimized for queries

---

**Version:** 1.0
**Last Updated:** November 2025
