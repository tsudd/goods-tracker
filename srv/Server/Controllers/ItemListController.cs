using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodsTracker.Platform.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemListController : ControllerBase
{
    private readonly ILogger<ItemListController> _logger;
    private readonly IItemManager _itemManager;

    public ItemListController(
        IItemManager itemManager,
        ILogger<ItemListController> logger)
    {
        _logger = logger;
        _itemManager = itemManager;
    }

    [HttpGet]
    // [Authorize]
    public async Task<IEnumerable<BaseItemModel>?> GetItems(int index, string orderBy, int shop, bool onlyDiscount)
    {
        try
        {
            var token = this.Request.Cookies["access_token"];
            return await _itemManager.GetBaseItemsPage(index, orderBy, shop, onlyDiscount);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"couldn't return items page: {ex.Message}");
            return null;
        }
    }

    [HttpGet("search")]
    public async Task<IEnumerable<BaseItemModel>?> SearchItems(
        int index,
        string q,
        string orderBy,
        int shop,
        bool onlyDiscount)
    {
        try
        {
            return await _itemManager.SearchItems(index, q, orderBy, shop, onlyDiscount);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"couldn't return items page: {ex.Message}");
            return null;
        }
    }

    [HttpGet("info")]
    public async Task<InfoModel> GetInfo()
    {
        return await _itemManager.GetItemsInfoAsync();
    }
}