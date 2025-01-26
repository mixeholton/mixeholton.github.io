namespace Komit.Sandbox.App.State;
public class WorkItemState: StateBase
{
    public string Title { get; set; }
    public Guid BoardId { get; set; }
    public BoardState Board { get; set; }
}
