namespace GoodsTracker.Platform.Server.Controllers.Extensions;

using Microsoft.AspNetCore.Mvc;

internal static class ControllerBaseExtension
{
    internal static string? ReadUserFromTokenOrDefault(this ControllerBase controller)
    {
        return controller.User.Claims.FirstOrDefault(static claim => claim.Type == "user_id")
                         ?.Value;
    }
}
