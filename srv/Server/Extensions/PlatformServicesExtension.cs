using GoodsTracker.Platform.Server.Modules;
using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Server.Services.DbAccess;
using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class PlatformServicesExtension
{
    public static void AddPlatformServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbAccess, HanaDbAccess>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<IItemManager, ItemManager>();

        // services.AddLocalization();
    }
}