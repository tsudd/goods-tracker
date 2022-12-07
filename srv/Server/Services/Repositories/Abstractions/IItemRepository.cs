using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;

namespace GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

public interface IItemRepository
{
    Task<BaseInfo> GetItemsInfoAsync();
    Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(
        int page,
        int amount,
        ItemsOrder order,
        int vendorFilterId,
        bool discountOnly,
        string? q = null);
}