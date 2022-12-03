using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoodsTracker.Platform.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemListController : ControllerBase
{
    private readonly ILogger<ItemListController> _logger;
    private readonly IItemManager _itemManager;

    public ItemListController(IItemManager itemManager, ILogger<ItemListController> logger)
    {
        _logger = logger;
        _itemManager = itemManager;
    }

    [HttpGet]
    public Task<IEnumerable<BaseItemModel>> GetItems(int page, int months = 1, int amount = 30)
    {
        return Task.FromResult<IEnumerable<BaseItemModel>>(Enumerable.Empty<BaseItemModel>().ToArray());
    }

    [HttpGet("count")]
    public Task<int> GetCount()
    {
        return _itemManager.GetAmountOfItemsAsync();
    }
}