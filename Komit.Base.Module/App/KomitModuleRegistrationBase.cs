using System.Collections.ObjectModel;

namespace Komit.Base.Module.App;
public abstract class KomitModuleRegistrationBase
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Schema { get; }
    protected abstract List<PermissionValue> DefinePermissionInfo();
    private ReadOnlyCollection<PermissionValue> _permissions;
    public ReadOnlyCollection<PermissionValue> GetPermissions() => _permissions ??= new ReadOnlyCollection<PermissionValue>(DefinePermissionInfo());
    // Consider some kind of permission migration registration
}
