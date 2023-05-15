using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodsTracker.Platform.Server.Controllers;

// TODO: replace templates with constants for routes in Shared
// TODO: introduce response types in attributes
// TODO: make better responses according to HTTP protocol
[ApiController]
[Route("[controller]")]
public class ItemListController : ControllerBase
{
    private readonly ILogger<ItemListController> _logger;
    private readonly IItemManager _itemManager;

    public ItemListController(IItemManager itemManager, ILogger<ItemListController> logger)
    {
        this._logger = logger;
        this._itemManager = itemManager;
    }

    [HttpGet]
    public async Task<IEnumerable<BaseItemModel>?> GetItems(int index, string orderBy, int shop, bool onlyDiscount)
    {
        try
        {
            return await this._itemManager.GetBaseItemsPage(
                index, orderBy, shop, this.ReadUserFromTokenOrDefault(),
                onlyDiscount);
        }
        catch (InvalidOperationException ex)
        {
            this._logger.LogWarning($"couldn't return items page: {ex.Message}");

            return null;
        }
    }

    [HttpGet("search")]
    public async Task<IEnumerable<BaseItemModel>?> SearchItems(
        int index, string q, string orderBy, int shop,
        bool onlyDiscount)
    {
        try
        {
            return await this._itemManager.SearchItems(
                index, q, orderBy, shop,
                this.ReadUserFromTokenOrDefault(), onlyDiscount);
        }
        catch (InvalidOperationException ex)
        {
            this._logger.LogWarning($"couldn't return items page: {ex.Message}");

            return null;
        }
    }

    [HttpGet("info")]
    public async Task<InfoModel> GetInfo()
    {
        return await this._itemManager.GetItemsInfoAsync();
    }

    [Authorize]
    [HttpPost("like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostItemLike([FromBody] ItemLikeModel itemLike)
    {
        try
        {
            string? userId = this.ReadUserFromTokenOrDefault();

            if (userId == null)
            {
                throw new InvalidOperationException("user can't be empty when saving item like");
            }

            return await this._itemManager.LikeItem(itemLike.ItemId, userId) ? this.Ok() : this.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            this._logger.LogWarning($"couldn't save item like: {ex.Message}");

            return this.BadRequest();
        }
    }

    [Authorize]
    [HttpDelete("like/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteItemLike(int id)
    {
        try
        {
            string? userId = this.ReadUserFromTokenOrDefault();

            if (userId == null)
            {
                throw new InvalidOperationException("user can't be empty when saving item like");
            }

            return await this._itemManager.UnLikeItem(id, userId) ? this.Ok() : this.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            this._logger.LogWarning($"couldn't save item like: {ex.Message}");

            return this.BadRequest();
        }
    }

    private string? ReadUserFromTokenOrDefault()
    {
        return this.User.Claims.FirstOrDefault(claim => claim.Type == "user_id")
                   ?.Value;
    }
}
