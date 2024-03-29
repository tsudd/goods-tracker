using GoodsTracker.Platform.Shared.Models;

namespace GoodsTracker.Platform.Server.Modules.Item.Abstractions;

using FluentResults;

public interface IItemManager
{
    Task<InfoModel> GetItemsInfoAsync();

    Task<IEnumerable<BaseItemModel>> GetBaseItemsPage(
        int page, string order, int shopFilterId, string? userId,
        bool discountOnly = false);

    Task<IEnumerable<BaseItemModel>> SearchItems(
        int page, string q, string order, int shopFilterId,
        string? userId, bool discountOnly = false);

    Task<IEnumerable<BaseItemModel>> SearchItems(string q, string? userId);
    Task<bool> LikeItem(int itemId, string userId);
    Task<bool> UnLikeItem(int itemId, string userId);
    Task<Result<ItemModel>> GetItemAsync(int itemId);
}
