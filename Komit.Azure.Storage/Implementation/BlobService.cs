using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Komit.Azure.Storage.Implementation;
public class BlobService : IBlobCommandService
{
    protected string ConnectionString { get; } // Configuration["AzureBlobStorageOptions:ConnectionString"]
    protected string ContainerId { get; }
    public BlobService(string connectionString, string containerId)
    {
        ConnectionString = connectionString;
        ContainerId = containerId;
    }
    private BlobContainerClient GetTenantClient
        => new(ConnectionString, ContainerId);
    private BlobClient GetBlobClient(BlobLocation blob)
        => GetTenantClient.GetBlobClient(blob.BlobPath);
    private async Task<string> GetContentType(BlobClient blobClient)
    {
        var blobProperties = await blobClient.GetPropertiesAsync();
        return blobProperties.Value.ContentType;
    }
    public async Task<List<string>> List(BlobFolderLocation location)
    {
        var path = location.FolderPath;
        var blobs = new List<string>();
        await foreach (BlobItem blobItem in GetTenantClient.GetBlobsAsync(prefix: path))
        {
            blobs.Add(blobItem.Name.Replace(path + "/", ""));
        }
        return blobs;
    }
    public async Task<BlobStream> Get(BlobLocation location)
    {
        var blobClient = GetBlobClient(location);
        var stream = await blobClient.OpenReadAsync();
        var contentType = await GetContentType(blobClient);
        return new BlobStream(stream, contentType, location.BlobName);
    }
    public async Task Add(Stream stream, BlobLocation location)
    {
        await GetTenantClient.UploadBlobAsync(location.BlobPath, stream);
    }
    public async Task Remove(BlobLocation location)
    {
        await GetBlobClient(location).DeleteIfExistsAsync();
    }

    public async Task RemoveFolder(BlobFolderLocation folderLocation)
    {
        var tenantClient = GetTenantClient;
        await foreach (BlobItem blobItem in tenantClient.GetBlobsAsync(prefix: folderLocation.FolderPath))
        {
            await tenantClient.GetBlobClient(blobItem.Name).DeleteIfExistsAsync();
        }
    }
    public async Task<bool> Exists(BlobFolderLocation location)
    {
        var path = location.FolderPath;
        await foreach (BlobItem blobItem in GetTenantClient.GetBlobsAsync(prefix: path))
        {
            return true;
        }
        return false;
    }
}