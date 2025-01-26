namespace Komit.Sandbox.App.Commands;
public class UserCommandHandler : BaseWriteHandler<SandboxCommandContext>,
    ICommandHandler<CreateUserCommand>
{
    public UserCommandHandler(SandboxCommandContext context, ISessionService session) : base(context, session)
    {
    }
    public async Task Perform(CreateUserCommand command)
    {
        if (await Context.State.Users.AnyAsync(x => x.Name == command.Name))
            throw new DomainException("Brugeren kunne ikke oprettes", $"En anden bruger med navnet {command.Name} eksisterer allerede");
        Context.Users.Create().New(command.Name);
    }
}