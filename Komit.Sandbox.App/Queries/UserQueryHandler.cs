namespace Komit.Sandbox.App.Queries;

public class UserQueryHandler : BaseReadHandler<ISandboxQueryContext>,
    IQueryHandler<ShowUsersQuery, UserInfoDto[]>
{
    public UserQueryHandler(ISandboxQueryContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task<UserInfoDto[]> Perform(ShowUsersQuery query)
        => await Context.Users.Select(x => new UserInfoDto(x.Id, x.Name, x.CreatedDateTime, x.CreatedDate, x.CreatedTime)).ToArrayAsync();
}
