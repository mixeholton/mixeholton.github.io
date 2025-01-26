namespace Komit.Sandbox.App.Commands
{
    public class CycleCommandHandler : BaseWriteHandler<SandboxCommandContext>, ICommandHandler<AddCycleCommand>
    {
        public CycleCommandHandler(SandboxCommandContext context, ISessionService session) : base(context, session)
        {
        }

        public async Task Perform(AddCycleCommand command)
        {
            Context.Cycle.Create().New(command.Brand, command.color, command.Size);
        }
    }
}
