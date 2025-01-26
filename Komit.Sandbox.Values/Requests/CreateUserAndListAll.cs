using Komit.Sandbox.Values.Queries;

namespace Komit.Sandbox.Values.Requests;
public record CreateUserAndListAll(string Name): RequestBase<UserInfoDto[]>("Opret bruger og vis alle");

