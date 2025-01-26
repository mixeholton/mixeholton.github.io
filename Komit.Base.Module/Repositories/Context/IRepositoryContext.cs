using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Handlers.Context;

namespace Komit.Base.Module.Repositories.Context;
public interface IRepositoryContext: IAggregateContext, IAggregateCollectionContext
{
    KomitDbContext DbContext { get; }
}
