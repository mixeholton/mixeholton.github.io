namespace Komit.Sandbox.App.Queries
{
    public class CycleQueryHandler : BaseReadHandler<ISandboxQueryContext>,
        IQueryHandler<ShowCycleListQuery, IEnumerable<CycleInfo>>
    {
        public CycleQueryHandler(ISandboxQueryContext context, ISessionService session) : base(context, session)
        {
        }

        public async Task<IEnumerable<CycleInfo>> Perform(ShowCycleListQuery query)
        {
            return await Context.Cycle.Select(x => new CycleInfo(x.Id, x.Brand, x.Color, x.Size)).ToListAsync();
        }
    }
}
