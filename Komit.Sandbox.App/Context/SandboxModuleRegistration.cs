using Komit.Base.Module.App;
using Komit.Base.Values.Cqrs;

namespace Komit.Sandbox.App.Context;
public class SandboxModuleRegistration : KomitModuleRegistrationBase
{
    public override string Name => "Sandbox";
    public override string Description => "Its a sandbox";
    public override string Schema => "sandbox";
    protected override List<PermissionValue> DefinePermissionInfo() => [];
}
