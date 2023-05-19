using GoodsTracker.Platform.DB.Context;
using GoodsTracker.Platform.Server.Modules.Item;
using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

using GoodsTracker.Platform.Server.Modules.Favorites;

internal static class PlatformServicesExtension
{
    internal static void AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        options.Authority = configuration["FIREBASE_AUTHORITY"];

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = configuration["FIREBASE_AUTHORITY"],
                            ValidateAudience = true,
                            ValidAudience = configuration["FIREBASE_AUDIENCE"],
                            ValidateLifetime = true,
                        };
                    });

        services.AddDbContext<GoodsTrackerPlatformDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("POSTGRES_CONNECTION_STRING")));

        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IItemManager, ItemManager>();
        services.AddFavoritesModuleService();
    }
}
