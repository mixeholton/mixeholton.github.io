using Komit.Base.Dev.Server;
using Komit.Base.Module.App;
using Komit.Base.Ui.Components;
using Komit.Base.Values.Cqrs;
using Komit.BoxOfSand.App.Context;
using Komit.BoxOfSand.Module;
using Komit.BoxOfSand.Values.Commands;
using Komit.Sandbox.App.Context;
using Komit.Sandbox.Module;
using Komit.Sandbox.Web.Components;
using Komit.Sandbox.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllersWithViews();
builder.Services.AddDevServer(builder.Configuration);
builder.Services.AddDefaultAppConfiguration();
builder.Services.AddSandboxApp();
builder.Services.AddBoxOfSandApp();
builder.Services.AddBaseUiComponents();


builder.Services.AddTransient<IExceptionInspector, ExceptionInspector>();
builder.Services.AddHttpClient(Options.DefaultName);

var app = builder.Build();

app.UseDevServer();
await app.EnsureDatabaseMigration<SandboxDbContext>();
await app.EnsureDatabaseMigration<BoxOfSandDbContext>();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ISetSession>().Set(new SessionValue(Guid.NewGuid(), 1, Guid.NewGuid(), 1, Guid.NewGuid(), [], []));
    if ((await scope.ServiceProvider.GetRequiredService<IBoxOfSandQueryContext>().Books.AnyAsync()) == false)
    {
        var komit = scope.ServiceProvider.GetRequiredService<IKomitApp>();
        await komit.Perform(new CreateBookCommand("Carrie", "Carrie is a 1974 horror novel, the first by American author Stephen King. Set in Chamberlain, Maine, the plot revolves around Carrie White, a friendless, bullied high-school girl from an abusive religious household who discovers she has telekinetic powers.", null));
        await komit.Perform(new CreateBookCommand("'Salem's Lotrfonr", "'Salem's Lot is a 1975 horror novel by American author Stephen King. It was his second published novel. The story involves a writer named Ben Mears who returns to the town of Jerusalem's Lot (or 'Salem's Lot for short) in Maine, where he lived from the age of five through nine, only to discover that the residents are becoming vampires.", null));
        await komit.Perform(new CreateBookCommand("Jerusalem's Lot", "\"Jerusalem's Lot\" is a short story by Stephen King, first published in King's 1978 collection Night Shift.[1] The story was also printed in the illustrated 2005 edition of King's 1975 novel 'Salem's Lot.", "Night Shift"));
        await komit.Perform(new CreateBookCommand("Graveyard Shift", "\"Graveyard Shift\" is a short story by Stephen King, first published in the October 1970 issue of Cavalier magazine and collected in King's 1978 collection Night Shift. It was adapted into a 1990 film of the same name.", "Night Shift"));
        await komit.Perform(new CreateBookCommand("The Shining", "The Shining centers on Jack Torrance, a struggling writer and recovering alcoholic who accepts a position as the off-season caretaker of the historic Overlook Hotel in the Colorado Rockies.", null));
        await komit.Perform(new CreateBookCommand("The Long Walk", "Set in a future dystopian America, ruled by a totalitarian regime, the plot revolves around the contestants of a grueling annual walking contest.", null));
        await komit.Perform(new CreateBookCommand("The Mist", "The Mist is a psychological horror novella by American author Stephen King. First published by Viking Press in 1980 as part of the Dark Forces anthology, an edited version was subsequently included in King's 1985 collection Skeleton Crew.", "Skeleton Crew"));
        await komit.Perform(new CreateBookCommand("The Monkey", "The story begins with two young brothers, Peter and Dennis, finding a cymbal-banging monkey toy in the attic of their great uncle's house. Soon, it is revealed how their father, Hal, discovered the toy monkey inside an antique chest owned by his father (Hal's father was a merchant mariner who disappeared under mysterious circumstances.", "Skeleton Crew"));
        await komit.Perform(new CreateBookCommand("Pet Sematary", "Pet Sematary is a 1983 horror novel by American writer Stephen King. The novel was nominated for a World Fantasy Award for Best Novel in 1984.", null));
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapOpenApi();

app.UseAntiforgery();
app.UseAuthorization();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
    typeof(Komit.Base.Dev.Client._Imports).Assembly,
    typeof(Komit.Sandbox.Web.Client._Imports).Assembly,
    typeof(Komit.BoxOfSand.Ui.Components._Imports).Assembly);

app.Run();
namespace Komit.Sandbox.Web
{
    public partial class Program { }
}