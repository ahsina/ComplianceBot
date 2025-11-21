using AutoMapper;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using MediatR;

namespace ComplianceBot.Application.Reports.Commands.CreateReport;

/// <summary>
/// Handler for CreateReportCommand
/// </summary>
public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, ReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateReportCommandHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ReportDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            Type = request.Type,
            Status = ReportStatus.Draft,
            ReportingPeriod = request.ReportingPeriod,
            FiscalYear = request.FiscalYear,
            DueDate = request.DueDate,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.Email
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReportDto>(report);
    }
}
