using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Komit.Base.Module.Handlers.Context;
public abstract class KomitDbContext : DbContext, IReadContext
{
    //public DbSet<ArchivationRecord> ArchivationRecords { get; set; }
    IQueryable<T> IReadContext.Query<T>() => Set<T>().AsNoTracking();
    public KomitDbContext(DbContextOptions options, ISessionService session): base(options)
    {
        _session = session;
    }
    protected ISessionService _session;
    protected virtual bool UseTenancyForModule { get; } = true;
    public bool IsSingleTenant => _session.Value.KeyValues.FirstOrDefault(x => x.Key == "Tenancy")?.Value == "Single";
    public int TenantId => _session?.TenantId ?? 0;
    /// <summary>
    /// Enforces global query filer for IsArchived on StateBase
    /// </summary>
    //protected abstract bool UseArchivingForModule { get; }
    //private bool _showOnlyArchived = false;
    ///// <summary>
    ///// Notice this is set to false in the ctor of base read & write handlers 
    ///// To ensure that it must be manually set in each handler to see archived
    ///// </summary>
    //public bool ShowOnlyArchived
    //{
    //    get => UseArchivingForModule && _showOnlyArchived;
    //    set
    //    {
    //        if (value && !UseArchivingForModule)
    //            throw new InvalidOperationException("UseArchivingForModule is false");
    //        _showOnlyArchived = value;
    //    }
    //}
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapState(modelBuilder);
        ApplyModelConfigurations(modelBuilder);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeConverter>();
        configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>();
        configurationBuilder.Properties<TimeOnly>().HaveConversion<TimeOnlyConverter>();
    }
    protected abstract void MapState(ModelBuilder map);
    // This list is used because query filters can only be applied to root state types
    // This will allow inheritance for state types sharing a root state, like accounts in the accounting app
    private static readonly List<Type> _baseStateTypes = new() { typeof(StateBase), typeof(EntityBase) };
    public static void AddBaseStateType<T>() where T : StateBase
        => _baseStateTypes.Add(typeof(T));
    private void ApplyModelConfigurations(ModelBuilder modelBuilder)
    {
        // Consider Looking at "Shared-type entity types" https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations for base db config
        Expression<Func<StateBase, bool>> filter = default;
        if (UseTenancyForModule)
            filter = x => IsSingleTenant || x.TenantId == TenantId;
        //if (UseArchivingForModule && UseTenancyForModule)
        //    filter = x => (IsSingleTenant || x.TenantId == TenantId) && x.IsArchived == ShowOnlyArchived;
        //if (!UseArchivingForModule && UseTenancyForModule)
        //    filter = x => IsSingleTenant || x.TenantId == TenantId;
        //if (UseArchivingForModule && !UseTenancyForModule)
        //    filter = x => x.IsArchived == ShowOnlyArchived;
        if (filter == default)
            return;
        var stateTypes = modelBuilder.Model.GetEntityTypes().Where(x => x.ClrType.BaseType != null && _baseStateTypes.Contains(x.ClrType.BaseType));
        foreach (var state in stateTypes)
        {
            // Set queryFilter
            // Archivation should be based on states with IStateArchivable
            var parameter = Expression.Parameter(state.ClrType);
            var body = ReplacingExpressionVisitor.Replace(filter.Parameters.First(), parameter, filter.Body);
            var lambdaExpression = Expression.Lambda(body, parameter);
            state.SetQueryFilter(lambdaExpression);
        }
    }
    public void EnsureTenancy()
    {
        if ((TenantId < 1 && !_session.IsAnonymous && !_session.Value.IsUnchosen()) 
            || ChangeTracker.Entries().Select(x => x.Entity as StateBase).Any(x => x != null && x.TenantId != TenantId && UseTenancyForModule))
            throw new InvalidOperationException(nameof(EnsureTenancy));
    }
    public sealed override int SaveChanges()
    {
        EnsureTenancy();
        return base.SaveChanges();
    }
    public sealed override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        EnsureTenancy();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public sealed override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        EnsureTenancy();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    public sealed override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureTenancy();
        return base.SaveChangesAsync(cancellationToken);
    }
    protected class DateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeConverter() : base(
            v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : v, 
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        { }
    }
    protected class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified),
                dateTime => DateOnly.FromDateTime(dateTime))
        { }
    }
    public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan))
        { }
    }
}
