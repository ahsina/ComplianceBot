using AutoMapper;
using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Application.Common.Mappings;
using ComplianceBot.Application.Reports.Commands.CreateReport;
using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ComplianceBot.Application.Tests.Reports.Commands;

public class CreateReportCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly IMapper _mapper;

    public CreateReportCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _tenantContextMock = new Mock<ITenantContext>();
        _currentUserMock = new Mock<ICurrentUserService>();

        // Setup AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        // Setup mocks
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(x => x.TenantId).Returns(tenantId);
        _currentUserMock.Setup(x => x.Email).Returns("test@example.com");
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesReport()
    {
        // Arrange
        var reports = new List<Report>();
        var mockReportsDbSet = CreateMockDbSet(reports);
        _contextMock.Setup(x => x.Reports).Returns(mockReportsDbSet.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => { /* Simulate save */ })
            .ReturnsAsync(1);

        var handler = new CreateReportCommandHandler(
            _contextMock.Object,
            _tenantContextMock.Object,
            _currentUserMock.Object,
            _mapper);

        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = "2024-01",
            FiscalYear = 2024,
            DueDate = new DateTime(2024, 2, 15),
            Notes = "Test report"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ReportType.RBE);
        result.ReportingPeriod.Should().Be("2024-01");
        result.Status.Should().Be(ReportStatus.Draft);
        result.TenantId.Should().Be(_tenantContextMock.Object.TenantId);

        _contextMock.Verify(x => x.Reports.Add(It.IsAny<Report>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsAuditFields()
    {
        // Arrange
        var reports = new List<Report>();
        var mockReportsDbSet = CreateMockDbSet(reports);
        _contextMock.Setup(x => x.Reports).Returns(mockReportsDbSet.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateReportCommandHandler(
            _contextMock.Object,
            _tenantContextMock.Object,
            _currentUserMock.Object,
            _mapper);

        var command = new CreateReportCommand
        {
            Type = ReportType.CEDR,
            ReportingPeriod = "2024-Q1"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();

        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        mockDbSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(data.Add);

        return mockDbSet;
    }
}
