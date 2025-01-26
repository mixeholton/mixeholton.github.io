namespace Komit.Azure.Storage;
public interface IBlobCommandService : IBlobQueryService
{
    Task Add(Stream stream, BlobLocation location);
    Task Remove(BlobLocation location);
    Task RemoveFolder(BlobFolderLocation folderLocation);
}
