using Komit.BoxOfSand.Module;
using Komit.Base.Module.App;
var configuration = new Dictionary<string, string>
        {
           {"ConnectionStrings:Tenant", "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=sandbox-dev-sql;Trusted_Connection=True;MultipleActiveResultSets=true"}
        };
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddInMemoryCollection(configuration);
builder.Services.AddBoxOfSandDb().AddMigrationSessionHandling();
builder.Build().Run();

