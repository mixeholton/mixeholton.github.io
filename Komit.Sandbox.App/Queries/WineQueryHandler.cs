
using Komit.Base.Module.Handlers;

namespace Komit.Sandbox.App.Queries;
public class WineQueryHandler : BaseReadHandler<ISandboxQueryContext>,
    IQueryHandler<ShowWineListQuery, IEnumerable<WineInfo>>
{
    public WineQueryHandler(ISandboxQueryContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task<IEnumerable<WineInfo>> Perform(ShowWineListQuery query)
    {
        return await Context.Wine.Select(x => new WineInfo(x.Id, x.Name)).ToListAsync();
    }
}
