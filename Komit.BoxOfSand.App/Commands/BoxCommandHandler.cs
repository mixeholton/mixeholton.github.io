using Komit.BoxOfSand.App.Context;
using Komit.BoxOfSand.Values.Commands;

namespace Komit.BoxOfSand.App.Commands;
public class BoxCommandHandler : BaseWriteHandler<BoxOfSandCommandContext>,
    ICommandHandler<CreateBoxCommand>,
    ICommandHandler<ArchiveBoxCommand>,
    ICommandHandler<UndoLastBoxArchivationCommand>
{
    public BoxCommandHandler(BoxOfSandCommandContext context, BoxOfSandDbContext dbContext, ISessionService session) : base(context, session)
    {
        DbContext = dbContext;
    }

    public BoxOfSandDbContext DbContext { get; }

    public async Task Perform(CreateBoxCommand command)
    {
        if (await Context.State.Boxes.AnyAsync(x => x.Name == command.Name))
            throw new DomainException("Brugeren kunne ikke oprettes", $"En anden bruger med navnet {command.Name} eksisterer allerede");
        Context.Boxes.Create().New(command.Name);
    }

    public async Task Perform(ArchiveBoxCommand command)
    {
        throw new NotImplementedException();
        //var archivation = await Context.ArchivationService(Session).Begin(command);
        //archivation.Archive(await DbContext.Boxes.SingleAsync(x => x.Id == command.BoxId));
    }

    public async Task Perform(UndoLastBoxArchivationCommand command)
    {
        throw new NotImplementedException();
        //DbContext.ShowOnlyArchived = true;
        //var latestArchivationId = await DbContext.Set<BoxState>().Where(x => x.IsArchived).MaxAsync(x => x.ArchivationId);
        //DbContext.ShowOnlyArchived = false;
        //await Context.ArchivationService(Session).UndoArchivation<BoxState>(latestArchivationId ?? 0).Perform(command);
    }
}