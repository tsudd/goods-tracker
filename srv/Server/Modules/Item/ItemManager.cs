using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;
using GoodsTracker.Platform.Shared.Models;

namespace GoodsTracker.Platform.Server.Modules;

internal class ItemManager : IItemManager
{
    private const int pageSize = 30;
    private const string defaultCurrency = "BYN";
    private const string defaultWeightUnit = "g";
    private const string defaultCountry = "Belarus";
    private const string defaultImgLink = "img/no_image.png";
    private const int shopModelColumns = 3;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger _logger;

    public ItemManager(IItemRepository itemRepository, ILogger<ItemManager> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<InfoModel> GetItemsInfoAsync()
    {
        var itemsInfo = await _itemRepository.GetItemsInfoAsync();
        try
        {
            return ItemManager.MapInfoModel(itemsInfo);
        }
        catch (FormatException)
        {
            _logger.LogError(
                $"Couldn't map items info to model: wrong format");
            throw new InvalidOperationException();
        }

    }

    public async Task<IEnumerable<BaseItemModel>> SearchItems(
        int startIndex,
        string q,
        string order,
        int shopFilterId,
        bool discountOnly = false)
    {
        var itemsOrder = GetItemsOrder(order);
        var baseItemsEntities = await _itemRepository.GetItemsByGroupsAsync(
            startIndex,
            pageSize,
            itemsOrder,
            shopFilterId,
            discountOnly,
            q);
        var itemModels = new List<BaseItemModel>();

        foreach (var itemEntity in baseItemsEntities)
        {
            try
            {
                itemModels.Add(ItemManager.MapBaseItemModel(itemEntity));
            }
            catch (FormatException)
            {
                _logger.LogError(
                    $"Couldn't map base item entity to entity model: some fields are missing in {itemEntity.Id}");
            }
        }
        return itemModels;
    }

    public async Task<IEnumerable<BaseItemModel>> GetBaseItemsPage(
        int page,
        string order,
        int shopFilterId,
        bool discountOnly = false)
    {
        var itemsOrder = GetItemsOrder(order);
        var baseItemsEntities =
            await _itemRepository.GetItemsByGroupsAsync(page, pageSize, itemsOrder, shopFilterId, discountOnly);
        var itemModels = new List<BaseItemModel>();

        foreach (var itemEntity in baseItemsEntities)
        {
            try
            {
                itemModels.Add(MapBaseItemModel(itemEntity));
            }
            catch (FormatException)
            {
                _logger.LogError(
                    $"Couldn't map base item entity to entity model: some fields are missing in {itemEntity.Id}");
            }
        }
        return itemModels;
    }

    private ItemsOrder GetItemsOrder(string orderString)
    {
        return orderString switch
        {
            "cheap" => ItemsOrder.CheapFirst,
            "expensive" => ItemsOrder.ExpensiveFirst,
            "date" => ItemsOrder.ByLastUpdateDate,
            var _ => ItemsOrder.None
        };
    }

    private static BaseItemModel MapBaseItemModel(BaseItem baseItemEntity)
    {
        return new BaseItemModel
        {
            Id = baseItemEntity.Id,
            Name = baseItemEntity.Name,
            Price = baseItemEntity.Price,
            DiscountPrice = baseItemEntity.DiscountPrice ?? 0,
            Discount = baseItemEntity.Discount ?? 0,
            OnDiscount = baseItemEntity.OnDiscount,
            Country = baseItemEntity.Country ?? defaultCountry,
            Currensy = baseItemEntity.Currensy ?? defaultCurrency,
            FetchDate = baseItemEntity.FetchDate ?? DateTime.Today,
            ImgLink = baseItemEntity.ImgLink ?? defaultImgLink,
            VendorName = baseItemEntity.VendorName ?? throw new FormatException(),
            Weight = baseItemEntity.Weight ?? 0,
            WeightUnit = baseItemEntity.WeightUnit ?? defaultWeightUnit,
        };
    }

    private static InfoModel MapInfoModel(BaseInfo baseInfo)
    {
        return new InfoModel
        {
            ItemsCount = baseInfo.ItemsCount,
            Shops = baseInfo.ShopsColumns
                .Select(shopColumns =>
                {
                    var columns = shopColumns.Split(',');
                    if (columns.Length != shopModelColumns)
                        throw new FormatException();
                    return new ShopModel
                    {
                        Id = int.Parse(columns[0]),
                        Name1 = columns[1],
                        Name2 = columns[2],
                    };
                })
        };
    }
}