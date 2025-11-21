using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ComplianceBot.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ComplianceBot.Infrastructure.Files;

/// <summary>
/// Azure Blob Storage implementation of IFileStorageService
/// </summary>
public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage")
            ?? throw new InvalidOperationException("AzureBlobStorage connection string not configured");

        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "compliancebot";
    }

    public async Task<string> UploadFileAsync(
        Guid tenantId,
        string path,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobPath = GetBlobPath(tenantId, path);
        var blobClient = containerClient.GetBlobClient(blobPath);

        await blobClient.UploadAsync(content, overwrite: true, cancellationToken);

        // Set content type
        await blobClient.SetHttpHeadersAsync(
            new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return blobPath;
    }

    public async Task<Stream> DownloadFileAsync(
        Guid tenantId,
        string path,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobPath = GetBlobPath(tenantId, path);
        var blobClient = containerClient.GetBlobClient(blobPath);

        var response = await blobClient.DownloadAsync(cancellationToken);
        return response.Value.Content;
    }

    public async Task DeleteFileAsync(
        Guid tenantId,
        string path,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobPath = GetBlobPath(tenantId, path);
        var blobClient = containerClient.GetBlobClient(blobPath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> GetTemporaryDownloadUrlAsync(
        Guid tenantId,
        string path,
        TimeSpan expiresIn,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobPath = GetBlobPath(tenantId, path);
        var blobClient = containerClient.GetBlobClient(blobPath);

        // Generate SAS token
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return await Task.FromResult(sasUri.ToString());
    }

    private string GetBlobPath(Guid tenantId, string path)
    {
        // Ensure tenant isolation: tenants/{tenantId}/{path}
        return $"tenants/{tenantId:N}/{path.TrimStart('/')}";
    }
}
