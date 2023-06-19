using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using GoodsTracker.Platform.Client;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using GoodsTracker.Platform.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(
    sp => new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    });

builder.Services.AddScoped<StateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(static s => s.GetRequiredService<StateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLocalization();
builder.Services.AddSingleton<UserAlertService>();
builder.Services.AddScoped<ItemManagementService>();
WebAssemblyHost host = builder.Build();

await host.SetDefaultCulture()
          .ConfigureAwait(false);

await host.RunAsync()
          .ConfigureAwait(false);
