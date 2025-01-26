namespace Komit.Base.Values;
public abstract record ValidatedValue
{
    public ValidatedValue()
    {
        if (!IsValid)
            throw new DomainException("Ugyldig værdi", GetType().Name);
    }
    public ValidatedValue(string title, string description)
    {
        if (!IsValid)
            throw new DomainException(title, description);
    }
    protected abstract bool IsValid { get; }
}