using GoodsTracker.Shared.Model;
namespace Server.Services;

public interface IDbSetup
{
    Task<IEnumerable<Item>> GetItemsAsync();
    Task<IEnumerable<Item>> GetPaginatedItemsAsync(DateTime date, int page = 1, int amount = 30);
    Task<int> GetItemsCountAsync();
    Task SetupDbAsync();
    string TestMethod();
}