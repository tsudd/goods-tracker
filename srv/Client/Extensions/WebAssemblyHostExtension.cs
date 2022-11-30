using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebAssemblyHostExtension
{
    public async static Task SetDefaultCulture(this WebAssemblyHost host)
    {
        var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
        var result = await localStorage.GetItemAsStringAsync("BlazorCulture");
        CultureInfo culture;
        if (result != null)
        {
            culture = new CultureInfo(result);
        }
        else
        {
            culture = new CultureInfo("en-US");
            await localStorage.SetItemAsStringAsync("BlazorCulture", "en-US");
        }
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}