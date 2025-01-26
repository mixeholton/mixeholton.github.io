
namespace Komit.Sandbox.App.Commands;
public class BoardCommandHandler : BaseWriteHandler<SandboxCommandContext>,
    ICommandHandler<AddWorkItemCommand>
{
    public BoardCommandHandler(SandboxCommandContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task Perform(AddWorkItemCommand command)
    {
        var board = (await Context.Boards.GetAll()).SingleOrDefault();
        if(board == null) 
        {
            board = Context.Boards.Create();
            //await SaveWithRollback(command);
        }
        board.AddWorkItem(command.Title);
    }
}
