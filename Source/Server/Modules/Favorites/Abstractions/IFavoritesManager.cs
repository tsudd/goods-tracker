namespace GoodsTracker.Platform.Server.Modules.Favorites.Abstractions;

using FluentResults;

using GoodsTracker.Platform.Shared.Models;

public interface IFavoritesManager
{
    Task<Result<IEnumerable<BaseItemModel>>> GetFavoritesAsync(string userId);
}
