using Komit.Base.Module.App;
using Komit.Base.Values.Cqrs;

namespace Komit.BoxOfSand.App.Context;
public class BoxOfSandModuleRegistration : KomitModuleRegistrationBase
{
    public override string Name => "Box Of Sand";
    public override string Description => "Its a box of sand";
    public override string Schema => "BoxOfSand";
    protected override List<PermissionValue> DefinePermissionInfo() => [];
}
