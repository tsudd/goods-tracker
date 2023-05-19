using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;
using GoodsTracker.Platform.Shared.Models;

namespace GoodsTracker.Platform.Server.Modules.Item;

using GoodsTracker.Platform.Server.Modules.Extensions;

internal sealed class ItemManager : IItemManager
{
    private const int pageSize = 30;
    private const int shopModelColumns = 3;
    private readonly IItemRepository itemRepository;
    private readonly ILogger<ItemManager> logger;

    public ItemManager(IItemRepository itemRepository, ILogger<ItemManager> logger)
    {
        this.itemRepository = itemRepository;
        this.logger = logger;
    }

    public async Task<InfoModel> GetItemsInfoAsync()
    {
        BaseInfo itemsInfo = await this.itemRepository.GetItemsInfoAsync().ConfigureAwait(false);

        try
        {
            return MapInfoModel(itemsInfo);
        }
        catch (FormatException ex)
        {
            LoggerMessage.Define(
                LogLevel.Error, 0,
                "Couldn't map items info to model: wrong format")(
                this.logger, null);

            throw new InvalidOperationException(ex.Message);
        }
    }

    public Task<IEnumerable<BaseItemModel>> SearchItems(
        int page, string q, string order, int shopFilterId,
        string? userId, bool discountOnly = false)
    {
        return this.GetBaseItemsAsync(
            page, order, shopFilterId, userId,
            discountOnly, $"%{q}%");
    }

    public Task<IEnumerable<BaseItemModel>> GetBaseItemsPage(
        int page, string order, int shopFilterId, string? userId,
        bool discountOnly = false)
    {
        return this.GetBaseItemsAsync(
            page, order, shopFilterId, userId,
            discountOnly);
    }

    private async Task<IEnumerable<BaseItemModel>> GetBaseItemsAsync(
        int page,
        string order,
        int shopFilterId,
        string? userId,
        bool discountOnly,
        string? q = null)
    {
        ItemsOrder itemsOrder = GetItemsOrder(order);

        IEnumerable<BaseItem> baseItemsEntities = await this.itemRepository.GetItemsByGroupsAsync(
            page, pageSize, itemsOrder, shopFilterId,
            discountOnly, userId, q).ConfigureAwait(false);

        return baseItemsEntities.Select(static i => i.ToModel());
    }

    public async Task<bool> LikeItem(int itemId, string userId)
    {
        if (!this.itemRepository.HasAll(itemId))
        {
            return false;
        }

        DateTime dateTime = DateTime.Now.ToUniversalTime();

        return await this.itemRepository.AddUserFavoriteItemAsync(itemId, userId, dateTime).ConfigureAwait(false);
    }

    public Task<bool> UnLikeItem(int itemId, string userId)
    {
        return this.itemRepository.DeleteUserFavoriteItemAsync(itemId, userId);
    }

    private static ItemsOrder GetItemsOrder(string orderString)
    {
        return orderString switch
        {
            "cheap" => ItemsOrder.CheapFirst,
            "expensive" => ItemsOrder.ExpensiveFirst,
            "date" => ItemsOrder.ByLastUpdateDate,
            var _ => ItemsOrder.None,
        };
    }

    private static InfoModel MapInfoModel(BaseInfo baseInfo)
    {
        return new InfoModel
        {
            ItemsCount = baseInfo.ItemsCount,
            Shops = baseInfo.ShopsColumns.Select(
                static shopColumns =>
                {
                    string[] columns = shopColumns.Split(',');

                    if (columns.Length != shopModelColumns)
                    {
                        throw new FormatException();
                    }

                    return new ShopModel
                    {
                        Id = int.Parse(columns[0]),
                        Name1 = columns[1],
                        Name2 = columns[2],
                    };
                }),
        };
    }
}
