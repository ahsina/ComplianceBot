using ComplianceBot.Application.Common.DTOs;
using MediatR;

namespace ComplianceBot.Application.Tenants.Queries.GetTenant;

/// <summary>
/// Query to get tenant information
/// </summary>
public record GetTenantQuery(Guid TenantId) : IRequest<TenantDto>;
