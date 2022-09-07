using Server.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DbSetupServiceExtension
{
    public static void AddHanaDbSetup(this IServiceCollection services)
    {
        services.AddSingleton<IDbSetup, HanaSetup>();
    }
}