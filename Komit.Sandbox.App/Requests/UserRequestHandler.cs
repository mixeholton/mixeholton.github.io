namespace Komit.Sandbox.App.Requests;
public class UserRequestHandler : BaseWriteHandler<SandboxCommandContext>,
    IRequestHandler<CreateUserAndListAll, UserInfoDto[]>
{
    public UserRequestHandler(SandboxCommandContext context, ISessionService session) : base(context, session)
    {
    }

    public async Task<UserInfoDto[]> Perform(CreateUserAndListAll request)
    {
        var test = Context.Test.Create().New(request.Name);
        await SaveWithRollback(request);
        var testTest = await Context.Test.GetAll();
        return await Context.State.Users.Select(x => new UserInfoDto(x.Id, x.Name, x.CreatedDateTime, x.CreatedDate, x.CreatedTime)).ToArrayAsync();
    }
}
