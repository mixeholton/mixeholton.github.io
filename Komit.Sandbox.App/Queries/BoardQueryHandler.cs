namespace Komit.Sandbox.App.Queries;
public class BoardQueryHandler : BaseReadHandler<ISandboxQueryContext>,
    IQueryHandler<ShowWorkItemsQuery, IEnumerable<WorkItemHeaderDto>>
{
    public BoardQueryHandler(ISandboxQueryContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task<IEnumerable<WorkItemHeaderDto>> Perform(ShowWorkItemsQuery query)
    {
        return await Context.Boards.SelectMany(x => x.WorkItems).Select(x => new WorkItemHeaderDto(x.Id, x.Title)).ToListAsync();
    }
}
