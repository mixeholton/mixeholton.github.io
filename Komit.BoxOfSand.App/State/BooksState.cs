namespace Komit.BoxOfSand.App.State
{
    public class BooksState : StateBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ?Collection {  get; set; }
    }
}
