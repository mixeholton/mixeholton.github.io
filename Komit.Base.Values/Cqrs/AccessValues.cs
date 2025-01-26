namespace Komit.Base.Values.Cqrs;
public record SessionValue(Guid Id, int Version, Guid CredentialsGuid, int TenantId, Guid AffiliationGuid, IEnumerable<KeyValue> KeyValues, IEnumerable<string> Permissions) : ValidatedValue
{
    public bool IsUnchosen() => Version == 0 && CredentialsGuid != default && AffiliationGuid == default && TenantId == default && !Permissions.Any() && !KeyValues.Any();
    public bool IsAnonymous() => Version == 0 && CredentialsGuid == default && AffiliationGuid == default && TenantId == default && !Permissions.Any() && !KeyValues.Any();
    protected override bool IsValid
        => Permissions != default && KeyValues != default && Id != default && Version >= 0 &&
        ((AffiliationGuid != default && TenantId > 0 && CredentialsGuid != default) || IsAnonymous() || IsUnchosen());
    public static SessionValue NewAnonymous()
        => new(Guid.NewGuid(), default, default, default, default, Enumerable.Empty<KeyValue>(), Enumerable.Empty<string>());
    public static SessionValue NewUnchosen(Guid credentialsGuid)
        => new(Guid.NewGuid(), default, credentialsGuid, default, default, Enumerable.Empty<KeyValue>(), Enumerable.Empty<string>()); 
    public bool HasPermission(string key)
        => Permissions.Contains(key);
    public bool HasAnyPermission(params string[] keys)
        => keys.Any(HasPermission);
    public bool HasAllPermissions(params string[] keys)
        => keys.All(HasPermission);
    public string GetKeyValue(string key)
        => KeyValues.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
}
public record PermissionValue(string Key, string Name, string Description);
public interface ISetSession
{
    void Set(SessionValue session);
}
// ToDo Move below to System module
public record GetPortalAccessRequest() : RequestBase<PortalAccessDto>("Hent bruger adgang");
public record SetPortalAccessCommand(Guid OrganizationId, Guid AffiliationId) : CommandBase("Set bruger adgang");
public record PortalAccessDto(string Name, Guid OrganizationId, Guid AffiliationId, IdNamePair[] OrganizationAffiliations, IdNamePair[] AffiliatedOrganizations)
{
    public IdNamePair GetOrganization() => AffiliatedOrganizations.FirstOrDefault(x => x.Id == OrganizationId) ?? new(default, "Ukendt");
    public IdNamePair GetAffiliation() => OrganizationAffiliations.FirstOrDefault(x => x.Id == AffiliationId) ?? new(default, "Ukendt");
    public static PortalAccessDto Unknown() => new("Ukendt", default, default, [], []);
    public static PortalAccessDto Unchosen() => new("Ikke valgt", default, default, [], []);
}
public record GetMyOrganizationAffiliations(Guid OrganizationId) : QueryBase<IEnumerable<IdNamePair>>("Hent mine adgange");