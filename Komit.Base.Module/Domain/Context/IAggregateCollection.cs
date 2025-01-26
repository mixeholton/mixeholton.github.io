namespace Komit.Base.Module.Domain.Context;
public interface IAggregateCollection
{
    void InitializeNew(IAggregateCollectionContext context);
}