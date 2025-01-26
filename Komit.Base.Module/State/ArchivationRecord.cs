namespace Komit.Base.Module.State;
public class ArchivationRecord : StateBase
{
    public Guid AffiliationGuid { get; set; }
    public string ArchiveAction { get; set; }
    public bool IsManaged { get; set; }
    public DateTime Time { get; set; }
}