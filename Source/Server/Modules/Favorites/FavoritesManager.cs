namespace GoodsTracker.Platform.Server.Modules.Favorites;

using FluentResults;

using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Modules.Extensions;
using GoodsTracker.Platform.Server.Modules.Favorites.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories;
using GoodsTracker.Platform.Shared.Models;

internal sealed class FavoritesManager : IFavoritesManager
{
    private readonly FavoritesRepository favoritesRepository;

    public FavoritesManager(FavoritesRepository favoritesRepository)
    {
        this.favoritesRepository = favoritesRepository;
    }

    public async Task<Result<IEnumerable<BaseItemModel>>> GetFavoritesAsync(string userId)
    {
        IEnumerable<BaseItem> favorites = await this.favoritesRepository.GetUserFavoritesAsync(userId).ConfigureAwait(false);

        return Result.Ok(favorites.Select(static f => f.ToModel()));
    }
}
