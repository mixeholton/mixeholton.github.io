using Komit.Base.Values.Cqrs;

namespace Komit.BoxOfSand.Values.Queries;
public record ShowBoxesQuery() : QueryBase<BoxInfoDto[]>(nameof(ShowBoxesQuery));
