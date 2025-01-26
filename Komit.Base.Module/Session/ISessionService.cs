using Komit.Base.Module.Session.Implementation;

namespace Komit.Base.Module.Session;
public interface ISessionService
{
    Guid AffiliationGuid { get; }
    int TenantId { get; }
    SessionValue Value { get; }
    public bool IsAnonymous { get; }
    public string GetValueFor(string key);
    bool HasPermission(string key);
    bool HasAnyPermission(params string[] keys);
    bool HasAllPermissions(params string[] keys);
}