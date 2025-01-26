
namespace Komit.Sandbox.App.State;
public class TestSubItemState: StateBase
{
    public string Name { get; set; }
    public Guid TestId { get; set; }
    public TestState Test { get; set; }
}
