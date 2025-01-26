namespace Komit.Sandbox.App.State;
public class TestState: UserState
{
    public string Test { get; set; }
    public ICollection<TestSubItemState> SubItems { get; set; } = new List<TestSubItemState>();
}
