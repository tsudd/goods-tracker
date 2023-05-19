namespace GoodsTracker.Platform.Server.Controllers;

using FluentResults;

using GoodsTracker.Platform.Server.Controllers.Extensions;
using GoodsTracker.Platform.Server.Modules.Favorites.Abstractions;
using GoodsTracker.Platform.Shared.Constants;
using GoodsTracker.Platform.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route($"{GoodsTrackerDefaults.GoodsTrackerApiV1}/{GoodsTrackerDefaults.FavoritesModuleRoute}")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoritesManager favoritesManager;

    public FavoritesController(IFavoritesManager favoritesManager)
    {
        this.favoritesManager = favoritesManager;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFavorites()
    {
        string? userId = this.ReadUserFromTokenOrDefault();

        if (userId is null)
        {
            return this.Forbid();
        }

        Result<IEnumerable<BaseItemModel>> getFavoritesResult = await this.favoritesManager.GetFavoritesAsync(userId)
                                                                          .ConfigureAwait(false);

        return getFavoritesResult.IsSuccess ? this.Ok(getFavoritesResult.Value) : this.Problem();
    }
}
