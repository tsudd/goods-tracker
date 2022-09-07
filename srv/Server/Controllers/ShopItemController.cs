using Microsoft.AspNetCore.Mvc;
using Server.Services;
using GoodsTracker.Shared.Model;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopItemController : ControllerBase
{
    private readonly ILogger<ShopItemController> _logger;
    private readonly IDbSetup _dbSetup;

    public ShopItemController(ILogger<ShopItemController> logger, IDbSetup dbSetup)
    {
        _logger = logger;
        _dbSetup = dbSetup;
    }

    [HttpGet]
    public async Task<IEnumerable<Item>> GetItems(int page, int months = 1, int amount = 30)
    {
        try
        {
            return (await _dbSetup.GetPaginatedItemsAsync(DateTime.Today.AddMonths(-months), page, amount)).ToArray();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("couldn't send items to the client.");
            return Enumerable.Empty<Item>().ToArray();
        }
    }

    [HttpGet("count")]
    public async Task<int> GetCount()
    {
        try
        {
            return await _dbSetup.GetItemsCountAsync();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("couldn't get the amount of items.");
            return 0;
        }
    }
}