using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;

namespace GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

internal interface IItemRepository
{
    Task<BaseInfo> GetItemsInfoAsync();

    Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(
        int page, int amount, ItemsOrder order, int vendorFilterId,
        bool discountOnly, string? userId = null, string? q = null);

    Task<bool> AddUserFavoriteItemAsync(int itemId, string userId, DateTime dateTime);
    Task<bool> DeleteUserFavoriteItemAsync(int itemId, string userId);
    bool HasAll(params int[] itemIds);
}
