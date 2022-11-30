namespace Microsoft.Extensions.DependencyInjection;

public static class PlatformMiddlewareExtension
{
    public static void UsePlatformMiddleware(this WebApplication app)
    {
        var supportedCultures = new[] { "en-US", "ru-RU" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);
    }
}