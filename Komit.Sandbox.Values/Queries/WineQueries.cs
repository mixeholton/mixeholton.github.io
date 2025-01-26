namespace Komit.Sandbox.Values.Queries;
public record ShowWineListQuery() : QueryBase<IEnumerable<WineInfo>>(nameof(ShowWineListQuery));
