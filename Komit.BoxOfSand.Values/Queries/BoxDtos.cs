namespace Komit.BoxOfSand.Values.Queries;
public record BoxInfoDto(Guid Id, string Name, IEnumerable<ContentInfoDto> Content);
public record ContentInfoDto(Guid Id, string Name);
