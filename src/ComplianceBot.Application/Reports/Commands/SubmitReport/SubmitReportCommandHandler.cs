using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Enums;
using ComplianceBot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Reports.Commands.SubmitReport;

/// <summary>
/// Handler for SubmitReportCommand
/// Submits the report to regulatory authority
/// </summary>
public class SubmitReportCommandHandler : IRequestHandler<SubmitReportCommand, SubmitReportResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUser;

    public SubmitReportCommandHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        ICurrentUserService currentUser)
    {
        _context = context;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
    }

    public async Task<SubmitReportResult> Handle(SubmitReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _context.Reports
            .FirstOrDefaultAsync(
                r => r.Id == request.ReportId && r.TenantId == _tenantContext.TenantId,
                cancellationToken);

        if (report == null)
        {
            throw new EntityNotFoundException(nameof(ComplianceBot.Domain.Entities.Report), request.ReportId);
        }

        // Validate report is ready for submission
        if (report.Status != ReportStatus.Validated)
        {
            throw new ValidationException("Report must be validated before submission");
        }

        // TODO: Implement actual submission to regulatory authority
        // For now, simulate submission
        var referenceNumber = $"{report.Type}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 20);

        report.Status = ReportStatus.Submitted;
        report.SubmittedAt = DateTime.UtcNow;
        report.SubmittedByUserId = _currentUser.UserId;
        report.RegulatoryReferenceNumber = referenceNumber;
        report.ModifiedAt = DateTime.UtcNow;
        report.ModifiedBy = _currentUser.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return new SubmitReportResult
        {
            ReportId = report.Id,
            RegulatoryReferenceNumber = referenceNumber,
            SubmittedAt = report.SubmittedAt.Value,
            Success = true
        };
    }
}
