namespace Komit.Azure.Storage;
public interface IBlobQueryService
{
    Task<BlobStream> Get(BlobLocation location);
    Task<List<string>> List(BlobFolderLocation location);
    Task<bool> Exists(BlobFolderLocation location);
}
