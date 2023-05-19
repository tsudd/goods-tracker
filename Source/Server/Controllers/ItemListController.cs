using GoodsTracker.Platform.Server.Modules.Item.Abstractions;
using GoodsTracker.Platform.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodsTracker.Platform.Server.Controllers;

// TODO: replace templates with constants for routes in Shared
// TODO: introduce response types in attributes
// TODO: make better responses according to HTTP protocol
// TODO: more validation
[ApiController]
[Route("[controller]")]
public class ItemListController : ControllerBase
{
    private readonly IItemManager itemManager;

    public ItemListController(IItemManager itemManager)
    {
        this.itemManager = itemManager;
    }

    [HttpGet]
    public async Task<IEnumerable<BaseItemModel>?> GetItems(int index, string orderBy, bool onlyDiscount, int? shop)
    {
        try
        {
            return await this.itemManager.GetBaseItemsPage(
                index, orderBy, shop ?? 0, this.ReadUserFromTokenOrDefault(),
                onlyDiscount).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    [HttpGet("search")]
    public async Task<IEnumerable<BaseItemModel>?> SearchItems(int index, string q, string orderBy, bool onlyDiscount, int? shop)
    {
        try
        {
            return await this.itemManager.SearchItems(
                index, q, orderBy, shop ?? 0,
                this.ReadUserFromTokenOrDefault(), onlyDiscount).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    [HttpGet("info")]
    public async Task<InfoModel> GetInfo()
    {
        return await this.itemManager.GetItemsInfoAsync().ConfigureAwait(false);
    }

    [Authorize]
    [HttpPost("like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostItemLike([FromBody] ItemLikeModel itemLike)
    {
        if (itemLike == null)
        {
            return this.BadRequest("Request model should be provided");
        }

        try
        {
            string? userId = this.ReadUserFromTokenOrDefault();

            if (userId == null)
            {
                throw new InvalidOperationException("user can't be empty when saving item like");
            }

            return await this.itemManager.LikeItem(itemLike.ItemId, userId).ConfigureAwait(false) ? this.Ok() : this.NotFound();
        }
        catch (InvalidOperationException)
        {
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

            return await this.itemManager.UnLikeItem(id, userId).ConfigureAwait(false) ? this.Ok() : this.NotFound();
        }
        catch (InvalidOperationException)
        {
            return this.BadRequest();
        }
    }

    private string? ReadUserFromTokenOrDefault()
    {
        return this.User.Claims.FirstOrDefault(claim => claim.Type == "user_id")
                   ?.Value;
    }
}
