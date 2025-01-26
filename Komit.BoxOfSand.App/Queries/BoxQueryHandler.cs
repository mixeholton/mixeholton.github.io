using Komit.BoxOfSand.App.Context;
using Komit.BoxOfSand.Values.Queries;

namespace Komit.BoxOfSand.App.Queries;

public class BoxQueryHandler : BaseReadHandler<IBoxOfSandQueryContext>,
    IQueryHandler<ShowBoxesQuery, BoxInfoDto[]>
{
    public BoxQueryHandler(IBoxOfSandQueryContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task<BoxInfoDto[]> Perform(ShowBoxesQuery query)
        => await Context.Boxes.Include(x => x.Content).Select(x => new BoxInfoDto(x.Id, x.Name, x.Content.Select(c => new ContentInfoDto(c.Id, c.Name)))).ToArrayAsync();
}
