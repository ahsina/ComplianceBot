namespace ComplianceBot.Domain.Enums;

/// <summary>
/// Types of compliance reports supported by the platform
/// Luxembourg financial regulatory reports
/// </summary>
public enum ReportType
{
    /// <summary>
    /// Recueil électronique de données - Monthly regulatory report
    /// </summary>
    RBE = 1,

    /// <summary>
    /// Central Electronic Database of Reporting - Annual report
    /// </summary>
    CEDR = 2,

    /// <summary>
    /// Foreign Account Tax Compliance Act - US tax reporting
    /// </summary>
    FATCA = 3,

    /// <summary>
    /// Common Reporting Standard - OECD tax reporting
    /// </summary>
    CRS = 4,

    /// <summary>
    /// Sustainable Finance Disclosure Regulation - ESG reporting
    /// </summary>
    SFDR = 5
}
