namespace Komit.Base.Module.Session.Implementation;
public class SessionService : ISessionService, ISetSession
{
    public Guid AffiliationGuid => Value.AffiliationGuid;
    public int TenantId => Value.TenantId;
    public SessionValue Value { get; private set; }
    public bool IsAnonymous => Value.IsAnonymous();
    public bool HasPermission(string key) 
        => Value.Permissions.Contains(key);
    public bool HasAnyPermission(params string[] keys)
        => keys.Any(HasPermission);
    public bool HasAllPermissions(params string[] keys)
        => keys.All(HasPermission);
    void ISetSession.Set(SessionValue session)
    {
        if (Value != null) 
            throw new DomainException("Ugyldig bruger styring", "Man kan ikke skifte brugeren for en handling");
        Value = session;
    }
    public string GetValueFor(string key) => Value?.KeyValues?.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
}
