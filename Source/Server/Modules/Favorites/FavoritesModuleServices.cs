namespace GoodsTracker.Platform.Server.Modules.Favorites;

using GoodsTracker.Platform.Server.Modules.Favorites.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories;

internal static class FavoritesModuleServices
{
    internal static void AddFavoritesModuleService(this IServiceCollection services)
    {
        services.AddScoped<IFavoritesManager, FavoritesManager>();
        services.AddScoped<FavoritesRepository>();
    }
}
