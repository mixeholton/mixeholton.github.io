namespace Komit.Sandbox.App.State;
public class BoardState: StateBase
{
    public ICollection<WorkItemState> WorkItems { get; set; } = new List<WorkItemState>();
}
