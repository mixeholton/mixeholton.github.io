namespace Komit.Sandbox.Values.Queries;
public record ShowUsersQuery(bool IncludeInactive = false): QueryBase<UserInfoDto[]>("Vis brugere");
