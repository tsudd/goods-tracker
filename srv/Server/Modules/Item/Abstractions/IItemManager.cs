using GoodsTracker.Platform.Shared.Models;

namespace GoodsTracker.Platform.Server.Modules.Item.Abstractions;

public interface IItemManager
{
    Task<InfoModel> GetItemsInfoAsync();
    Task<IEnumerable<BaseItemModel>> GetBaseItemsPage(int page, string order, int shopFilterId);
    Task<IEnumerable<BaseItemModel>> SearchItems(int page, string q, string order, int shopFilterId);
}