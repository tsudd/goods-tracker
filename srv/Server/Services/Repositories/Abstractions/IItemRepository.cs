using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;

namespace GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

public interface IItemRepository
{
    Task<int> GetItemCountAsync();
    Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(int page, int amount, ItemsOrder order, string? q = null);
}