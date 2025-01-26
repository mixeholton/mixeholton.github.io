using Komit.Base.Module.Domain.Context;
using Komit.Base.Module.Handlers.Context;
using Komit.Base.Module.Handlers.Context.Markers;
using System.Reflection;
namespace Komit.Base.Module.App;
public class ConventionChecker<TProgram, TDbContext, TModelContext, TReadContext>
       where TProgram : class
       where TDbContext : KomitDbContext, TReadContext
       where TModelContext : ModelContext<TDbContext, TReadContext>
       where TReadContext : IReadContext   
{
    private readonly Type iHandlerType = typeof(IHandlerMarker);
    private readonly Type readHandlerType = typeof(BaseReadHandler<TReadContext>);
    private readonly Type writeHandlerType = typeof(BaseWriteHandler<TModelContext>);
    private readonly AssemblyName[] _assemblyNames;
    private readonly List<Type> _types = new();
    private readonly string _productName;
    private readonly string _moduleName;
    private readonly string _appNamespace;
    private readonly string _valuesNamespace;
    private readonly List<Message> _deviations = new();
    public IEnumerable<Message> Deviations => _deviations;
    public ConventionChecker(string productName, string moduleName, string[] ignoredTypes = default, string[] ignoredNamespaces = default)
    {
        var apiAssembly = typeof(TProgram).Assembly;
        _productName = productName;
        _moduleName = moduleName;
        _assemblyNames = apiAssembly.GetReferencedAssemblies();
        _valuesNamespace = LoadRequiredAssemblyTypes("Values");
        _appNamespace = LoadRequiredAssemblyTypes("App");
        _types.RemoveAll(x => (ignoredTypes?.Contains(x.Name) ?? false) || (ignoredNamespaces?.Any(n => x.Namespace?.StartsWith(n) ?? false) ?? false));
        CheckProjectConventions(apiAssembly);
        CheckContextConventions();
        CheckHandlerConventions();
        CheckModelConventions();
        CheckCqreValuesConventions();
        CheckValuesConventions();
        _types.Clear();
        _assemblyNames = Array.Empty<AssemblyName>();
    }
    public void WriteDeviationsToConsole()
    {
        Console.WriteLine("Convention deviations checked, found: " + Deviations.Count());
        Deviations.ToList().ForEach(x => Console.WriteLine(x.Title + ": " + x.Details));
    }
    private void CheckProjectConventions(Assembly apiAssembly)
    {
        if (_productName.ToLower() != "komit" && _productName.ToLower() != "viggo")
            Deviation("Invalid product name", _productName);
        if (apiAssembly.GetName().Name != AssemblyName("Api"))
            Deviation("Invalid api name", $"Api project name: {apiAssembly.GetName().Name}, should be: {AssemblyName("Api")}");
    }
    private void CheckContextConventions()
    {
        void CheckContextTypeNaming(Type type, string nameEnding)
        {
            var typeName = _moduleName + nameEnding;
            var contextFolderNamespace = AppFolderNamespace("Context");
            if (type.Namespace != contextFolderNamespace || type.Name != typeName)
                Deviation($"Deviation for {nameEnding}: {type.FullName}", $"Should be {contextFolderNamespace}.{typeName}");
        }
        CheckContextTypeNaming(typeof(TDbContext), "DbContext");
        CheckContextTypeNaming(typeof(TModelContext), "ModelContext");
        var type = typeof(TReadContext);
        var nameEnding = "ReadContext";
        var typeName = "I" + _moduleName + nameEnding;
        var contextFolderNamespace = AppFolderNamespace("Context");
        if (type.Namespace != contextFolderNamespace || type.Name != typeName)
            Deviation($"Deviation for {nameEnding}: {type.FullName}", $"Should be {contextFolderNamespace}.{typeName}");
    }
    private void CheckHandlerConventions()
    {
        var handlers = TypesAssignableTo(iHandlerType);
        var nameEnding = "CommandHandler";
        var unknownWriteHandlers =
            GetUnknownTypes(GetUnknownTypes(GetUnknownTypes(TypesAssignableTo(writeHandlerType, handlers),
            nameEnding, AppFolderNamespace(nameEnding + "s")),
            nameEnding = "EventHandler", AppFolderNamespace(nameEnding + "s")),
            nameEnding = "RequestHandler", AppFolderNamespace(nameEnding + "s"));
        unknownWriteHandlers.ToList().ForEach(x => Deviation("Deviation for writehandler: " + x.FullName, 
            $"Should match {_appNamespace}.[Command|Event|Request]Handlers.[DomainName][Command|Event|Request]Handler"));
        nameEnding = "QueryHandler";
        var folderNamespace = AppFolderNamespace(nameEnding + "s");
        CheckTypeNaming(TypesAssignableTo(readHandlerType, handlers), nameEnding, folderNamespace);
    }
    private void CheckModelConventions()
    {
        var stateTypes = TypesAssignableTo(typeof(StateBase));
        var entities = TypesAssignableTo(typeof(EntityBase));
        CheckTypeNaming(entities, "Entity", AppFolderNamespace("Entities"));
        CheckTypeNaming(stateTypes.Where(x => !entities.Contains(x)), "State", AppFolderNamespace("State"));
        CheckTypeNaming(TypesAssignableTo(typeof(IAggregate)), "", AppFolderNamespace("Domain"));
        CheckTypeNaming(TypesAssignableTo(typeof(IAggregateCollection)), "", AppFolderNamespace("Domain"));
    }
    private void CheckCqreValuesConventions()
    {
        var actions = TypesAssignableTo(typeof(RequestBase));
        var commands = TypesAssignableTo(typeof(CommandBase), actions);
        var queries = TypesAssignableTo(typeof(QueryBase), actions);
        var events = TypesAssignableTo(typeof(EventBase), actions);
        var requests = actions.ToList();
        requests.RemoveAll(commands.Contains);
        requests.RemoveAll(queries.Contains);
        requests.RemoveAll(events.Contains);
        var nameEnding = "Command";
        CheckTypeNaming(commands, nameEnding, ValueFolderNamespace(nameEnding + "s"));
        CheckTypeNaming(queries, nameEnding = "Query", ValueFolderNamespace("Queries"));
        CheckTypeNaming(events, nameEnding = "Event", ValueFolderNamespace(nameEnding + "s"));
        CheckTypeNaming(requests, nameEnding = "Request", ValueFolderNamespace(nameEnding + "s"));
    }
    private void CheckValuesConventions()
    {
        var valueTypes = _types.Where(x => (x.Namespace?.StartsWith(_valuesNamespace) ?? false) && !x.IsAssignableTo(typeof(RequestBase))).ToList();
        var valueFolderNamespace = ValueFolderNamespace("Values");
        var values = valueTypes.Where(x => (x.Namespace?.StartsWith(valueFolderNamespace) ?? false) || x.Name.EndsWith("Value")).ToList();
        CheckTypeNaming(values, "Value", valueFolderNamespace);
        var dtoFolderNamespace = ValueFolderNamespace("Dtos");
        var dtos = valueTypes.Where(x => (x.Namespace?.StartsWith(dtoFolderNamespace) ?? false) || x.Name.EndsWith("Dto")).ToList();
        CheckTypeNaming(dtos, "Dto", dtoFolderNamespace);
        var unknownValueValueTypes = valueTypes.ToList();
        unknownValueValueTypes.RemoveAll(values.Contains);
        unknownValueValueTypes.RemoveAll(dtos.Contains);
        unknownValueValueTypes.ForEach(x => 
            Deviation($"Unknown value type: {x.FullName}",
            $"Should match a value type convention or the type name/namespace should be ignored for conventions"));
    }
    private void CheckTypeNaming(IEnumerable<Type> types, string typeNameEnding, string typeFolderNamespace)
        => types.ToList().ForEach(x => CheckTypeNaming(x, typeNameEnding, typeFolderNamespace));
    private void CheckTypeNaming(Type type, string typeNameEnding, string typeFolderNamespace)
    {
        if (!type.Namespace.StartsWith(typeFolderNamespace) || !type.Name.EndsWith(typeNameEnding))
            Deviation($"Deviation for {typeNameEnding ?? "Aggregate"}: {type.FullName}",
            $"Should match {typeFolderNamespace}.[DescriptiveName]{typeNameEnding}");
    }
    string AppFolderNamespace(string nameEnding)
        => $"{_appNamespace}.{nameEnding}";
    string ValueFolderNamespace(string nameEnding)
        => $"{_valuesNamespace}.{nameEnding}";
    private IEnumerable<Type> GetUnknownTypes(IEnumerable<Type> types, string typeNameEnding, string typeFolderNamespace)
        => types.Where(x => !x.Namespace.StartsWith(typeFolderNamespace) || !x.Name.EndsWith(typeNameEnding));
    private IEnumerable<Type> TypesAssignableTo(Type type, IEnumerable<Type>? types = default)
        => (types ?? _types).Where(x => x.IsAssignableTo(type));
    private void Deviation(string title, string details)
        => _deviations.Add(new Message(title, details));
    private string AssemblyName(string purpose) 
        => $"{_productName}.{_moduleName}.{purpose}";
    private string LoadRequiredAssemblyTypes(string purpose)
    {
        var assemblyName = _assemblyNames.FirstOrDefault(x => x.Name == AssemblyName(purpose));
        var assembly = assemblyName == null ? null : Assembly.Load(assemblyName);
        if (assembly == null)
            Deviation("Missing required assembly", AssemblyName(purpose));
        else
            _types.AddRange(assembly.DefinedTypes);
        return assemblyName?.Name ?? string.Empty;
    }
}
