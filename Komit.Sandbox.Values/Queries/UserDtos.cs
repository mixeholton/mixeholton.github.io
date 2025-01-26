namespace Komit.Sandbox.Values.Queries;
public record UserInfoDto(Guid Id, string Name, DateTime? CreatedDateTime, DateOnly? CreatedDate, TimeOnly? CreatedTime);
