# Contributing to ComplianceBot

Thank you for your interest in contributing to ComplianceBot! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and professional
- Provide constructive feedback
- Focus on what is best for the project

## Getting Started

1. **Fork the repository**
2. **Clone your fork:**
   ```bash
   git clone https://github.com/your-username/ComplianceBot.git
   ```
3. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Setup

### Prerequisites

- .NET 8 SDK
- SQL Server 2019+ or Docker
- Visual Studio 2022 / Rider / VS Code
- Git

### Local Environment

1. **Start dependencies with Docker:**
   ```bash
   docker-compose up -d
   ```

2. **Update database:**
   ```bash
   cd src/ComplianceBot.Infrastructure
   dotnet ef database update --startup-project ../ComplianceBot.API
   ```

3. **Run the API:**
   ```bash
   cd src/ComplianceBot.API
   dotnet run
   ```

## Coding Standards

### C# Style Guide

- **Naming Conventions:**
  - Classes, Methods, Properties: `PascalCase`
  - Variables, Parameters: `camelCase`
  - Private fields: `_camelCase`
  - Constants: `PascalCase`

- **File Organization:**
  - One class per file
  - File name matches class name
  - Maximum 200 lines per file
  - Maximum 50 lines per method

- **SOLID Principles:**
  - Single Responsibility
  - Open/Closed
  - Liskov Substitution
  - Interface Segregation
  - Dependency Inversion

### Code Quality

- **No compiler warnings**
- **All tests must pass**
- **Code coverage:** Aim for 80%+ on business logic
- **Use async/await** for all I/O operations
- **Avoid magic numbers:** Use constants or enums

### Example

```csharp
// Good
public class ReportService : IReportService
{
    private readonly IApplicationDbContext _context;
    private const int MaxReportAge = 365;

    public ReportService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Report> GetReportAsync(Guid id)
    {
        var report = await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
        {
            throw new EntityNotFoundException(nameof(Report), id);
        }

        return report;
    }
}
```

## Adding New Features

### 1. Domain Changes

If adding new entities:

```bash
# Add entity in src/ComplianceBot.Domain/Entities/
# Add configuration in src/ComplianceBot.Infrastructure/Persistence/Configurations/
```

### 2. Application Layer

Create CQRS commands/queries:

```bash
# Commands: src/ComplianceBot.Application/{Feature}/Commands/{CommandName}/
#   - {CommandName}Command.cs
#   - {CommandName}CommandHandler.cs
#   - {CommandName}CommandValidator.cs

# Queries: src/ComplianceBot.Application/{Feature}/Queries/{QueryName}/
#   - {QueryName}Query.cs
#   - {QueryName}QueryHandler.cs
```

### 3. API Endpoints

Add controller methods:

```csharp
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
public async Task<IActionResult> CreateFeature([FromBody] CreateFeatureCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetFeature), new { id = result.Id }, result);
}
```

### 4. Tests

Write tests for:
- Domain entities (unit tests)
- Command/Query handlers (unit tests)
- API endpoints (integration tests)

```bash
# Add tests in tests/{LayerName}.Tests/
```

## Database Migrations

### Creating Migrations

```bash
cd src/ComplianceBot.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../ComplianceBot.API
```

### Applying Migrations

```bash
dotnet ef database update --startup-project ../ComplianceBot.API
```

### Migration Guidelines

- **Descriptive names:** `AddReportValidationFields`
- **Test rollback:** Ensure migrations can be reverted
- **Review before merge:** All migrations reviewed by senior dev

## Testing

### Running Tests

```bash
# All tests
dotnet test

# Specific project
dotnet test tests/Application.Tests/

# With coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Writing Tests

**Unit Test Example:**

```csharp
public class CreateReportCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesReport()
    {
        // Arrange
        var context = new Mock<IApplicationDbContext>();
        var tenantContext = new Mock<ITenantContext>();
        var handler = new CreateReportCommandHandler(context.Object, tenantContext.Object);

        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = "2024-01"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ReportType.RBE);
    }
}
```

## Pull Request Process

### Before Submitting

1. **Update from main:**
   ```bash
   git fetch origin
   git rebase origin/main
   ```

2. **Run tests:**
   ```bash
   dotnet test
   ```

3. **Check for warnings:**
   ```bash
   dotnet build
   ```

### PR Guidelines

- **Title:** Clear, descriptive (e.g., "Add FATCA report validation")
- **Description:**
  - What changes were made
  - Why they were made
  - Testing performed
  - Related issues

- **Size:** Keep PRs small (< 500 lines changed)
- **Tests:** Include tests for new features
- **Documentation:** Update docs if needed

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added where needed
- [ ] Documentation updated
- [ ] No new warnings
- [ ] Tests pass locally
```

## Code Review

### As a Reviewer

- **Be constructive:** Suggest improvements, don't just criticize
- **Be timely:** Review within 24 hours
- **Focus on:**
  - Correctness
  - Security
  - Performance
  - Maintainability
  - Test coverage

### As an Author

- **Respond to feedback:** Address all comments
- **Ask questions:** If feedback is unclear
- **Don't take it personally:** Code review improves quality

## Release Process

### Version Numbering

We use Semantic Versioning (MAJOR.MINOR.PATCH):

- **MAJOR:** Breaking changes
- **MINOR:** New features (backward compatible)
- **PATCH:** Bug fixes

### Changelog

Update `CHANGELOG.md` with:
- New features
- Bug fixes
- Breaking changes

## Questions?

- **Slack:** #compliancebot-dev
- **Email:** dev@compliancebot.lu
- **Documentation:** See `/docs` folder

## License

By contributing, you agree that your contributions will be licensed under the project's proprietary license.

---

Thank you for contributing to ComplianceBot!
