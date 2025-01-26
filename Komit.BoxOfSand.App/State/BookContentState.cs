using Komit.BoxOfSand.App.Domain;

namespace Komit.BoxOfSand.App.State;

internal class BookContentState : StateBase
{
    public string Name { get; set; }
    public BooksState Book { get; set; }
    public Guid BookId { get; set; }
}
