using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Reports.Commands.UpdateReportStatus;

/// <summary>
/// Handler for UpdateReportStatusCommand
/// </summary>
public class UpdateReportStatusCommandHandler : IRequestHandler<UpdateReportStatusCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUser;

    public UpdateReportStatusCommandHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        ICurrentUserService currentUser)
    {
        _context = context;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
    {
        var report = await _context.Reports
            .FirstOrDefaultAsync(
                r => r.Id == request.ReportId && r.TenantId == _tenantContext.TenantId,
                cancellationToken);

        if (report == null)
        {
            throw new EntityNotFoundException(nameof(ComplianceBot.Domain.Entities.Report), request.ReportId);
        }

        report.Status = request.NewStatus;

        if (!string.IsNullOrEmpty(request.Notes))
        {
            report.Notes = request.Notes;
        }

        report.ModifiedAt = DateTime.UtcNow;
        report.ModifiedBy = _currentUser.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
