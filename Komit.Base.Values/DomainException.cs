namespace Komit.Base.Values;
public class DomainException : Exception
{
    public string Title { get; }
    public string Description { get; }
    public Message GetMessage => new(Title, Description);
    public DomainException(string title, string description) : base($"{title}: {description}")
    {
        Title = title;
        Description = description;
    }
}
public static class ExceptionExtensions
{
    public static Exception GetDeepestDomainException(this Exception exception)
        => exception is DomainException || exception.InnerException == default
            ? exception : exception.InnerException.GetDeepestDomainException();
}