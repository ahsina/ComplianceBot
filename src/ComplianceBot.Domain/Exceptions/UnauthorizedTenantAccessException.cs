namespace ComplianceBot.Domain.Exceptions;

/// <summary>
/// Exception thrown when trying to access resources from another tenant
/// Critical for multi-tenancy security
/// </summary>
public class UnauthorizedTenantAccessException : DomainException
{
    public UnauthorizedTenantAccessException(Guid attemptedTenantId, Guid actualTenantId)
        : base($"Unauthorized access attempt to tenant '{attemptedTenantId}'. Current tenant is '{actualTenantId}'.")
    {
    }

    public UnauthorizedTenantAccessException(string message) : base(message)
    {
    }
}
