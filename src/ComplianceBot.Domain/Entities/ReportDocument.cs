using ComplianceBot.Domain.Common;

namespace ComplianceBot.Domain.Entities;

/// <summary>
/// Document attached to a report (source data, generated XML, validation PDF, etc.)
/// </summary>
public class ReportDocument : TenantEntity
{
    public Guid ReportId { get; set; }

    /// <summary>
    /// Document type (e.g., "SourceData", "GeneratedXML", "ValidationReport", "SubmissionReceipt")
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// MIME type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Azure Blob Storage path
    /// </summary>
    public string BlobStoragePath { get; set; } = string.Empty;

    /// <summary>
    /// File hash for integrity verification
    /// </summary>
    public string? FileHash { get; set; }

    // Navigation property
    public Report? Report { get; set; }
}
