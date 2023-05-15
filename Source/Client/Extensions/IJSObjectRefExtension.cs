using Microsoft.JSInterop;

namespace GoodsTracker.Platform.Client.Extenstions;

public static class IJSObjectRefExtenstions
{
    public static async ValueTask InitializeInactivityTimer<T>(
        this IJSObjectReference module,
        DotNetObjectReference<T> dotNetObjectReference) where T : class
    {
        await module.InvokeVoidAsync("initializeInactivityTimer", dotNetObjectReference);
    }
}