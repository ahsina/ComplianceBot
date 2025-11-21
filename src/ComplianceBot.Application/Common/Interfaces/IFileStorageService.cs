namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// File storage service abstraction (Azure Blob Storage implementation)
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Upload a file to blob storage
    /// </summary>
    Task<string> UploadFileAsync(
        Guid tenantId,
        string path,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file from blob storage
    /// </summary>
    Task<Stream> DownloadFileAsync(
        Guid tenantId,
        string path,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from blob storage
    /// </summary>
    Task DeleteFileAsync(
        Guid tenantId,
        string path,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a temporary SAS URL for direct file access
    /// </summary>
    Task<string> GetTemporaryDownloadUrlAsync(
        Guid tenantId,
        string path,
        TimeSpan expiresIn,
        CancellationToken cancellationToken = default);
}
