using GoodsTracker.Platform.Server.Entities;

namespace GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

public interface IItemRepository
{
    Task<int> GetItemCountAsync();
    Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(int page, int amount);
}