using GoodsTracker.Platform.Shared.Models;

namespace GoodsTracker.Platform.Server.Modules.Item.Abstractions;

public interface IItemManager
{
    Task<int> GetAmountOfItemsAsync();
    Task<IEnumerable<BaseItemModel>> GetBaseItemsPage(int page);
}