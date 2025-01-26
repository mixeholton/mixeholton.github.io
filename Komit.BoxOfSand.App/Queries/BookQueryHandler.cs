using Komit.BoxOfSand.App.Context;
using Komit.BoxOfSand.Values.Queries;

namespace Komit.BoxOfSand.App.Queries;

public class BookQueryHandler : BaseReadHandler<IBoxOfSandQueryContext>, IQueryHandler<ShowBooksQuery, BooksInfoDto[]>
{
    public BookQueryHandler(IBoxOfSandQueryContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task<BooksInfoDto[]> Perform(ShowBooksQuery query)
    {
        var result = await Context.Books.Select(x => new BooksInfoDto(x.Id, x.Name, x.Description, x.Collection)).ToArrayAsync();
        return result;

    }

}
