namespace Komit.BoxOfSand.App.State;
public class ContentState: StateBase
{
    public string Name { get; set; }
    public Guid BoxId { get; set; }
    public BoxState Box { get; set; }
}
