namespace ComplianceBot.Domain.Enums;

/// <summary>
/// Subscription tiers for SaaS pricing
/// </summary>
public enum SubscriptionTier
{
    /// <summary>
    /// Trial period (14-30 days)
    /// </summary>
    Trial = 0,

    /// <summary>
    /// Basic plan - 1 report type, 5 users
    /// </summary>
    Basic = 1,

    /// <summary>
    /// Professional plan - 3 report types, 20 users
    /// </summary>
    Professional = 2,

    /// <summary>
    /// Enterprise plan - All report types, unlimited users, dedicated support
    /// </summary>
    Enterprise = 3,

    /// <summary>
    /// Suspended due to non-payment
    /// </summary>
    Suspended = 99
}
