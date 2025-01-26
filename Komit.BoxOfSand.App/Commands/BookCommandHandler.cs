using Komit.BoxOfSand.App.Context;
using Komit.BoxOfSand.Values.Commands;

namespace Komit.BoxOfSand.App.Commands;

public class BookCommandHandler : BaseWriteHandler<BoxOfSandCommandContext>, ICommandHandler<CreateBookCommand>
{
    public BookCommandHandler(BoxOfSandCommandContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task Perform(CreateBookCommand command)
    {
        if (await Context.State.Books.AnyAsync(x => x.Name == command.Name))
            throw new DomainException("Kan ikke oprette bogen", "Bogen findes allerede");
        Context.Books.Create().New(command);
    }
}
