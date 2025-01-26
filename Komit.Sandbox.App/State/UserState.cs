namespace Komit.Sandbox.App.State;
public class UserState : EntityBase
{
    public string Name { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateOnly? CreatedDate { get; set; }
    public TimeOnly? CreatedTime { get; set; }
}
