namespace ComplianceBot.Modules.RBE.Models;

/// <summary>
/// Data model for RBE (Recueil électronique de données) report
/// Luxembourg monthly regulatory report
/// </summary>
public class RBEReportData
{
    /// <summary>
    /// Reporting month (YYYY-MM)
    /// </summary>
    public string ReportingMonth { get; set; } = string.Empty;

    /// <summary>
    /// Total assets under management (AuM) in EUR
    /// </summary>
    public decimal TotalAssetsUnderManagement { get; set; }

    /// <summary>
    /// Number of funds managed
    /// </summary>
    public int NumberOfFunds { get; set; }

    /// <summary>
    /// Number of individual clients
    /// </summary>
    public int NumberOfClients { get; set; }

    /// <summary>
    /// Total transaction volume for the month
    /// </summary>
    public decimal TransactionVolume { get; set; }

    /// <summary>
    /// List of fund positions
    /// </summary>
    public List<FundPosition> FundPositions { get; set; } = new();
}

/// <summary>
/// Individual fund position
/// </summary>
public class FundPosition
{
    public string FundCode { get; set; } = string.Empty;
    public string FundName { get; set; } = string.Empty;
    public decimal NetAssetValue { get; set; }
    public int NumberOfShares { get; set; }
    public DateTime ValuationDate { get; set; }
}
