using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;

namespace GoodsTracker.Platform.Server.Modules;

internal class ItemManager : IItemManager
{
    private readonly IItemRepository _itemRepository;

    public ItemManager(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public Task<int> GetAmountOfItemsAsync()
    {
        return _itemRepository.GetItemCountAsync();
    }
}