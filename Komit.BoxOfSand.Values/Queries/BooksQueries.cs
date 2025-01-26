using Komit.Base.Values.Cqrs;

namespace Komit.BoxOfSand.Values.Queries;


public record ShowBooksQuery() : QueryBase<BooksInfoDto[]>(nameof(ShowBooksQuery));