using System.Globalization;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

namespace Microsoft.Extensions.DependencyInjection;

internal static class WebAssemblyHostExtension
{
    public static async Task SetDefaultCulture(this WebAssemblyHost host)
    {
        var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
        string? result = await localStorage.GetItemAsStringAsync("BlazorCulture").ConfigureAwait(false);
        CultureInfo culture;

        if (result != null)
        {
            culture = new CultureInfo(result);
        }
        else
        {
            culture = new CultureInfo("en-US");
            await localStorage.SetItemAsStringAsync("BlazorCulture", "en-US").ConfigureAwait(false);
        }

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
