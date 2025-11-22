namespace ComplianceBot.Application.Common.Models;

/// <summary>
/// Result of a report file generation operation
/// </summary>
public class ReportFileResult
{
    /// <summary>
    /// Indicates if file generation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Path to the generated file (Azure Blob Storage URL or local path)
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// File name
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// File format (e.g., "XML", "CSV", "PDF")
    /// </summary>
    public string? FileFormat { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Content type / MIME type
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";

    /// <summary>
    /// Indicates if this is a sandbox file (not for real submission)
    /// </summary>
    public bool IsSandbox { get; set; }

    /// <summary>
    /// Error message if generation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the file was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
