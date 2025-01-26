using Komit.Base.Dev.Client;
using Komit.Base.Ui.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
await builder.Services.AddDevClient(builder.HostEnvironment.BaseAddress);
builder.Services.AddBaseUiComponents();
await builder.Build().RunAsync();
