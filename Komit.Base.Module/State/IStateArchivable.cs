namespace Komit.Base.Module.State;
public interface IStateArchivable
{
    bool IsArchived { get; set; }
    Guid? ArchivationId { get; set; }
}
