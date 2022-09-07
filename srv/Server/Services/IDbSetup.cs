using GoodsTracker.Shared.Model;
namespace Server.Services;

public interface IDbSetup
{
    Task<IEnumerable<Item>> GetItemsAsync();
    Task<IEnumerable<Item>> GetItemsAsync(DateTime date);
    Task<int> GetItemsCountAsync();
    Task SetupDbAsync();
    string TestMethod();
}