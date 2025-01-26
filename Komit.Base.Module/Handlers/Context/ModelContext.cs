using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Handlers.Context.Transaction;
using Komit.Base.Module.Repositories.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Komit.Base.Module.Handlers.Context;
public class ModelContext<TDbContext, TReadContext> : ITransactionService, IRepositoryContext
    where TDbContext : KomitDbContext, TReadContext
    where TReadContext : IReadContext
{
    private IDbContextTransaction? _transaction;
    private IList<(StateBase state, IAggregate aggregate)> _aggregateCreationResults;
    private List<(StateBase state, Func<EventBase> @event)> _events;
    private IList<Message> _messages;
    protected TDbContext Context { get; }
    public bool InSubScope { get; set; }
    //public ArchivationService ArchivationService(ISessionService session) => new(this, this, session);
    KomitDbContext IRepositoryContext.DbContext => Context;
    public TReadContext State => Context;
    //bool IWriteContext.ShowOnlyArchived { get => State.ShowOnlyArchived; set => State.ShowOnlyArchived = value; }
    public ModelContext(TDbContext context)
    {
        Context = context;
        Reset();
    }
    private void Reset()
    {
        InSubScope = false;
        _transaction = default;
        _messages = [];
        _events = [];
        _aggregateCreationResults = [];
    }
    private CommandResult BuildCommandResult(RequestBase command)
    {
        return new(true, _aggregateCreationResults.Any()
                ? _aggregateCreationResults.Select(x => new IdNamePair(x.state.Id, x.aggregate.CreationCodeName)).ToArray() 
                : [],
            command.GetDescription(), _messages);
    }
    void IAggregateContext.PublishEvent<TState>(IExposeState<TState> source, Func<EventBase> @event) => _events.Add((source._state, @event));
    void IAggregateContext.AddMessage(Message message) => _messages.Add(message);
    private record AggregateCreationForState<TState>(TState State, Func<TState, string> CreationNameFunction) : IAggregate
      where TState : StateBase, new()
    {
        public string CreationCodeName => CreationNameFunction(State);
        public void InitializeNew(IAggregateContext context) => throw new InvalidOperationException();
    }
    void IAggregateContext.RegisterCreatedState<TState>(IExposeState<TState> entity)
    {
        entity._state.Id = Guid.CreateVersion7();
        entity._state.TenantId = Context.TenantId;
        _aggregateCreationResults.Add((entity._state, entity));
    }
    void IAggregateContext.RegisterCreatedState<TState>(TState state, Func<TState, string>? creationNameFunction)
    {
        state.Id = Guid.CreateVersion7();
        state.TenantId = Context.TenantId;
        if(creationNameFunction != null)
            _aggregateCreationResults.Add((state, new AggregateCreationForState<TState>(state, creationNameFunction)));
    }

    void IAggregateCollectionContext.AddCreatedState<TState>(TState state)
    {
        state.Id = Guid.CreateVersion7();
        state.TenantId = Context.TenantId;
        Context.Set<TState>().Add(state);
    }
    void IAggregateCollectionContext.AddCreatedState<TState>(IExposeState<TState> entity)
        => ((IAggregateCollectionContext)this).AddCreatedState(entity._state);
    void IAggregateCollectionContext.RemoveEntity<TState>(TState entity) => Context.Set<TState>().Remove(entity);
    // Transaction Handling
    async Task<IEnumerable<EventBase>> ITransactionService.PublishEvents()
    {
        List<EventBase> events = [];
        if (_events.Any())
        {
            await (this as ITransactionService).EnsureTransactionScope();
            await Context.SaveChangesAsync();
            _events.ForEach(x => 
            {
                var @event = x.@event();
                @event.SetSourceId(x.state.Id);
                events.Add(@event);
            });
            _events.Clear();
        }
        return events;
    }
    public record BatchEvent<T>(IEnumerable<T> Events) : EventBase(nameof(BatchEvent<T>));
    async Task ITransactionService.EnsureTransactionScope()
    {
        if (_transaction == default)
            _transaction = await Context.Database.BeginTransactionAsync();
    }
    async Task<CommandResult> IWriteContext.SaveWithRollback(RequestBase command)
    {
        var transactionService = this as ITransactionService;
        await transactionService!.EnsureTransactionScope();
        return await transactionService.SaveCurrentChanges(command);
    }
    async Task<CommandResult> ITransactionService.SaveCurrentChanges(RequestBase command)
    {
        await Context.SaveChangesAsync();
        var result = BuildCommandResult(command);
        if (_transaction == default && !InSubScope)
            Reset();
        return result;
    }
    async Task<CommandResult> ITransactionService.Commit(RequestBase command)
    {
        await Context.SaveChangesAsync();
        if (_transaction != default && !InSubScope)
            await _transaction.CommitAsync();
        var result = BuildCommandResult(command);
        if (!InSubScope)
            Reset();
        return result;
    }
    async Task<CommandResult> ITransactionService.Rollback(RequestBase command)
    {
        if (_transaction != default && !InSubScope)
            await _transaction.RollbackAsync();
        Context.ChangeTracker.Clear();
        var result = BuildCommandResult(command);
        if (!InSubScope)
            Reset();
        return result;
    }
}