using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Domain.Enums;
using MediatR;

namespace ComplianceBot.Application.Tenants.Commands.CreateTenant;

/// <summary>
/// Command to create a new tenant (organization)
/// Only accessible by SystemAdmin
/// </summary>
public record CreateTenantCommand : IRequest<TenantDto>
{
    public string Name { get; init; } = string.Empty;
    public string CompanyRegistrationNumber { get; init; } = string.Empty;
    public string VATNumber { get; init; } = string.Empty;
    public string Country { get; init; } = "LU";
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string PrimaryContactName { get; init; } = string.Empty;
    public string PrimaryContactEmail { get; init; } = string.Empty;
    public string PrimaryContactPhone { get; init; } = string.Empty;
    public SubscriptionTier SubscriptionTier { get; init; }
    public List<ReportType> EnabledReportTypes { get; init; } = new();
}
