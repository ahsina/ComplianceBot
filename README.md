# ComplianceBot

**Automated Compliance Reporting Platform for Luxembourg Financial Sector**

ComplianceBot is a SaaS B2B multi-tenant platform that automates the generation, validation, and submission of regulatory compliance reports for the Luxembourg financial sector.

## Overview

- **Industry:** Financial Services (Luxembourg)
- **Architecture:** Modular Monolith with Clean Architecture + DDD
- **Multi-tenancy:** Schema-based isolation
- **Tech Stack:** ASP.NET Core 8, Entity Framework Core, Azure
- **Supported Reports:** RBE, CEDR, FATCA, CRS, SFDR

## Architecture

### High-Level Structure

```
┌─────────────────────────────────────┐
│       CLIENT TIER                    │
│  Web App (Blazor/React) + Mobile    │
└──────────────┬──────────────────────┘
               │ HTTPS/TLS
┌──────────────▼──────────────────────┐
│       API GATEWAY                    │
│  • JWT Auth                          │
│  • Rate Limiting                     │
│  • Versioning                        │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│    BACKEND SERVICES                  │
│  • Identity Service                  │
│  • Tenant Service                    │
│  • Reporting Modules                 │
│  • File Service                      │
│  • Notification Service              │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   DATA PROCESSING                    │
│  • Message Queue (Azure Service Bus) │
│  • Background Workers (Hangfire)     │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│       STORAGE                        │
│  • SQL Server (tenant data)          │
│  • Azure Blob (files)                │
│  • Redis (cache)                     │
└──────────────────────────────────────┘
```

### Project Structure

```
ComplianceBot/
├── src/
│   ├── ComplianceBot.Domain/          # Domain entities, value objects, enums
│   ├── ComplianceBot.Application/      # CQRS commands/queries, DTOs, interfaces
│   ├── ComplianceBot.Infrastructure/   # EF Core, persistence, external services
│   ├── ComplianceBot.Modules/
│   │   ├── RBE/                        # Recueil électronique de données
│   │   ├── CEDR/                       # Central Electronic Database of Reporting
│   │   ├── FATCA/                      # Foreign Account Tax Compliance Act
│   │   ├── CRS/                        # Common Reporting Standard
│   │   ├── SFDR/                       # Sustainable Finance Disclosure Regulation
│   │   └── Shared/                     # Shared module utilities
│   ├── ComplianceBot.API/              # REST API (ASP.NET Core)
│   └── ComplianceBot.Worker/           # Background jobs (Hangfire)
└── tests/
    ├── Domain.Tests/
    ├── Application.Tests/
    └── IntegrationTests/
```

## Key Features

### Multi-Tenancy
- **Schema-based isolation:** Each tenant has a separate database schema (`tenant_{TenantId}`)
- **Global query filters:** Additional security layer
- **Tenant context middleware:** Extracts tenant from JWT

### CQRS Pattern
- **Commands:** Mutations (create, update, delete)
- **Queries:** Read operations (optimized with Dapper)
- **MediatR:** Command/query bus
- **FluentValidation:** Request validation pipeline

### Security
- **JWT Authentication:** Token-based auth
- **Permission-based Authorization:** Fine-grained access control
- **Audit Logging:** All changes tracked (10-year retention)
- **Data Encryption:** At rest (Azure Storage) and in transit (TLS 1.3)

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server 2019+ or Azure SQL
- Azure Storage Account (or Azurite emulator)
- Visual Studio 2022 or Rider

### Database Setup

1. Update connection string in `appsettings.Development.json`
2. Run migrations:

```bash
cd src/ComplianceBot.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../ComplianceBot.API
dotnet ef database update --startup-project ../ComplianceBot.API
```

### Running the Application

**API:**
```bash
cd src/ComplianceBot.API
dotnet run
```

API will be available at: `https://localhost:5001`
Swagger UI: `https://localhost:5001/swagger`

**Worker:**
```bash
cd src/ComplianceBot.Worker
dotnet run
```

### Running Tests

```bash
dotnet test
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=ComplianceBot;",
    "AzureBlobStorage": "DefaultEndpointsProtocol=https;..."
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "ComplianceBot",
    "Audience": "ComplianceBot-Client"
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Development, Staging, Production
- `ConnectionStrings__DefaultConnection`: Database connection
- `Jwt__Key`: JWT secret key (use Azure Key Vault in production)

## Development Guidelines

### Code Standards

- **Naming:** PascalCase for classes/methods, camelCase for variables
- **Maximum file length:** 200 lines
- **Maximum method length:** 50 lines
- **Follow SOLID principles**

### Git Workflow

1. Create feature branch: `feature/report-validation`
2. Commit with clear messages
3. Create pull request
4. Code review required
5. Merge to `main`

### Adding a New Report Type

1. Create module in `src/ComplianceBot.Modules/{ReportType}/`
2. Add entity/DTOs in Domain/Application
3. Create commands/queries
4. Implement validators
5. Add API endpoints
6. Write tests

## API Documentation

### Authentication

All endpoints require JWT token in header:
```
Authorization: Bearer {token}
```

### Example Endpoints

**Create Report:**
```http
POST /api/v1/reports
Content-Type: application/json

{
  "type": "RBE",
  "reportingPeriod": "2024-01",
  "fiscalYear": 2024,
  "dueDate": "2024-02-15"
}
```

**Get Report:**
```http
GET /api/v1/reports/{id}
```

## Deployment

### Azure Resources Required

- **App Service:** 2+ instances (B2 tier minimum)
- **Azure SQL Database:** S3+ tier
- **Azure Blob Storage:** Standard tier
- **Azure Service Bus:** Standard tier
- **Azure Key Vault:** For secrets
- **Application Insights:** Monitoring

### CI/CD Pipeline

Uses Azure DevOps:
1. Build → Test → Publish artifacts
2. Deploy to Staging → Smoke tests
3. Manual approval
4. Deploy to Production → Monitor

## Performance Targets

- API Response: < 200ms (p95)
- Report Generation: < 2 minutes
- File Upload: Support 50MB files
- Concurrent Users: 1,000+

## Monitoring

- **Application Insights:** Request tracking, exceptions, performance
- **Serilog:** Structured logging
- **Health Checks:** `/health` endpoint

## Support

- Documentation: [Internal Wiki]
- Issues: [GitHub Issues]
- Email: support@compliancebot.lu

## License

Proprietary - Copyright © 2025 ComplianceBot S.à r.l.

---

**Version:** 1.0
**Last Updated:** November 2025
