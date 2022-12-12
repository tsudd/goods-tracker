using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoodsTracker.Platform.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly ILogger<ItemController> _logger;
    private readonly IItemManager _itemManager;

    public ItemController(IItemManager itemManager, ILogger<ItemController> logger)
    {
        _logger = logger;
        _itemManager = itemManager;
    }

    [HttpPost]
    public async Task<bool> PostItemLike([FromBody] ItemLikeModel itemLike)
    {
        try
        {
            if (itemLike.UserId == null)
                throw new InvalidOperationException("user can't be empty when saving item like");
            return await _itemManager.LikeItem(itemLike.ItemId, itemLike.UserId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"couldn't save item like: {ex.Message}");
            return false;
        }
    }
}