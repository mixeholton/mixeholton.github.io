namespace Komit.Sandbox.App.Commands;
public class WineCommandHandler : BaseWriteHandler<SandboxCommandContext>,
    ICommandHandler<AddWineCommand>
{
    public WineCommandHandler(SandboxCommandContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task Perform(AddWineCommand command)
    {
        Context.Wine.Create().New(command.Name);
    }
}
