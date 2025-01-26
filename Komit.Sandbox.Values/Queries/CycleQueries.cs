namespace Komit.Sandbox.Values.Queries
{
    public record ShowCycleListQuery() : QueryBase<IEnumerable<CycleInfo>>(nameof(ShowCycleListQuery));
}
