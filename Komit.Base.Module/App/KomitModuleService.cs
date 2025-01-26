namespace Komit.Base.Module.App;
public class KomitModuleService
{
    private readonly List<KomitModuleRegistrationBase> _modules = [];
    public IEnumerable<KomitModuleRegistrationBase> Modules => _modules;
    public KomitModuleService RegisterModule(KomitModuleRegistrationBase module)
    {
        _modules.Add(module);
        return this;
    }
}
