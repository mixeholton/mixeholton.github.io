namespace Komit.Sandbox.Values.Commands
{
    public record AddCycleCommand(string Brand, string color, int Size) : CommandBase(nameof(AddWineCommand));

}
