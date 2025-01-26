using Komit.BoxOfSand.App.State;
using Komit.BoxOfSand.Values.Commands;

namespace Komit.BoxOfSand.App.Domain;

public class Book : AggregateBase<BooksState>
{
    protected override string _creationCodeName => State.Name;

    protected override Func<IQueryable<BooksState>, IQueryable<BooksState>>? IncludedRelations => null;

    public Book New(CreateBookCommand command)
    {
        EnsureIsNew();
        State.Name = command.Name;
        State.Description = command.Description;
        State.Collection = command.Collection;
        return this;
    }
}
