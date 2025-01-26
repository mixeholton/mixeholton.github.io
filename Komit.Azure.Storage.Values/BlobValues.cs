namespace Komit.Azure.Storage.Values;
public record BlobLocation(string BlobName, params string[] FolderPathParts)
    : BlobFolderLocation(FolderPathParts)
{
    public string BlobPath => FolderPath + "/" + BlobName;
}
public record BlobFolderLocation(params string[] FolderPathParts)
{
    public BlobLocation GetBlobLocation(string blobName) => new(blobName, FolderPathParts);
    public string FolderPath => string.Join("/", FolderPathParts);// ?? Array.Empty<string>());
}
public record BlobStream(Stream Stream, string ContentType, string Name);
