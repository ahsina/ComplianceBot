using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Enums;
using ComplianceBot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Reports.Commands.DeleteReport;

/// <summary>
/// Handler for DeleteReportCommand
/// </summary>
public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteReportCommandHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Unit> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _context.Reports
            .FirstOrDefaultAsync(
                r => r.Id == request.ReportId && r.TenantId == _tenantContext.TenantId,
                cancellationToken);

        if (report == null)
        {
            throw new EntityNotFoundException(nameof(ComplianceBot.Domain.Entities.Report), request.ReportId);
        }

        // Only allow deletion of Draft reports
        if (report.Status != ReportStatus.Draft)
        {
            throw new ValidationException("Only draft reports can be deleted");
        }

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
