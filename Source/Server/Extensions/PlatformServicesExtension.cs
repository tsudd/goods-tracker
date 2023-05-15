using GoodsTracker.Platform.Server.Modules;
using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Server.Services.DbAccess;
using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class PlatformServicesExtension
{
    public static void AddPlatformServices(this IServiceCollection services)
    {
        services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable("FIREBASE_AUTHORITY");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("FIREBASE_AUTHORITY"),
                        ValidateAudience = true,
                        ValidAudience = Environment.GetEnvironmentVariable("FIREBASE_AUDIENCE"),
                        ValidateLifetime = true
                    };
                });

        services.AddSingleton<IDbAccess, HanaDbAccess>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<IItemManager, ItemManager>();

        // services.AddLocalization();
    }
}