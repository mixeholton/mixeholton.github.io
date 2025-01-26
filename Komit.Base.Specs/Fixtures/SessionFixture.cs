
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Komit.Base.Specs.Fixtures;
public class SessionFixture
{
    private static int _currentTenantId = 1;
    public int NextTenantId => _currentTenantId++;
    public SessionValue DefaultSession { get; protected set; }
    public IDistributedCache Cache { get; }
    public SessionFixture(IDistributedCache cache)
    {
        DefaultSession = SessionValue.NewAnonymous();
        Cache = cache;
    }
    public Task<Guid> SetDefaultSession(SessionValue session)
        => Add(DefaultSession = session);
    public Task<Guid> NewAdjustFromDefault(
        Guid? CredentialsId = null,
        Guid? AffiliationId = null,
        IEnumerable<KeyValue>? Keyvalues = null,
        IEnumerable<string>? Permissions = null)
    {
        var session = new SessionValue(
            Guid.NewGuid(),
            DefaultSession.Version+1,
            CredentialsId ?? DefaultSession.CredentialsGuid,
            DefaultSession.TenantId,
            AffiliationId ?? DefaultSession.AffiliationGuid,
            Keyvalues ?? DefaultSession.KeyValues,
            Permissions ?? DefaultSession.Permissions);
        return Add(session);
    }
    public Task<Guid> NewTenantAdjustFromDefault(
        Guid? CredentialsId = null,
        Guid? AffiliationId = null,
        IEnumerable<KeyValue>? Keyvalues = null,
        IEnumerable<string>? Permissions = null)
    {
        var newTenantId = NextTenantId;
        var session = new SessionValue(
            Guid.NewGuid(),
            DefaultSession.Version + 1,
            CredentialsId ?? DefaultSession.CredentialsGuid,
            newTenantId,
            AffiliationId ?? DefaultSession.AffiliationGuid,
            Keyvalues ?? DefaultSession.KeyValues,
            Permissions ?? DefaultSession.Permissions);
        return Add(session);
    }
    public virtual async Task<Guid> Add(SessionValue session)
    {
        await Cache.SetStringAsync(session.Id.ToString(), JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromHours(1)
        });
        return session.Id;
    }
    public virtual async Task<SessionValue> Get(Guid sessionId)
    {
        var sessionString = await Cache.GetStringAsync(sessionId.ToString());
        return string.IsNullOrWhiteSpace(sessionString) 
            ? SessionValue.NewAnonymous() with { Id = sessionId } 
            : JsonSerializer.Deserialize<SessionValue>(sessionString);
    }
    public virtual async Task Remove(Guid sessionId)
        => await Cache.RemoveAsync(sessionId.ToString());
}
