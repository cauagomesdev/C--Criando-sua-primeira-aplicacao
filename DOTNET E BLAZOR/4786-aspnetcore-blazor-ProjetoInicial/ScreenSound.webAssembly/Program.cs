using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ScreenSound.webAssembly;
using ScreenSound.webAssembly.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient<ArtistasAPI>();
builder.Services.AddTransient<MusicasAPI>();

builder.Services.AddHttpClient(
    "API",
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["APIServer"]);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }
);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();
