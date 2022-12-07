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
    public async Task<IEnumerable<BaseItemModel>?> GetItems(int index, string orderBy)
    {
        try
        {
            return await _itemManager.GetBaseItemsPage(index, orderBy);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"couldn't return items page: {ex.Message}");
            return null;
        }
    }

    [HttpGet("search")]
    public async Task<IEnumerable<BaseItemModel>?> SearchItems(int index, string q, string orderBy)
    {
        try
        {
            return await _itemManager.SearchItems(index, q, orderBy);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"couldn't return items page: {ex.Message}");
            return null;
        }
    }

    [HttpGet("count")]
    public async Task<int> GetCount()
    {
        return await _itemManager.GetAmountOfItemsAsync();
    }
}