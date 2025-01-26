
namespace Komit.BoxOfSand.App.State;
public class BoxState : StateBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<ContentState> Content { get; set; } = new List<ContentState>();
}
