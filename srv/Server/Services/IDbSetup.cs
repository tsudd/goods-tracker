using Shared.Model;
namespace Server.Services;

public interface IDbSetup
{
    Task<IEnumerable<Item>> GetItemsAsync();
    Task<IEnumerable<Item>> GetIemsAsync(DateTime date);
    Task SetupDbAsync();
    string TestMethod();
}