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
    public async Task<IEnumerable<Item>> GetItems()
    {
        try
        {
            return (await _dbSetup.GetItemsAsync()).ToArray();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("couldn't send items to the client.");
            return Enumerable.Empty<Item>().ToArray();
        }
    }
}