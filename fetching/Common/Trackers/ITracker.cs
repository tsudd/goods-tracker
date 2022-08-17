using Models;
using Common.Configs;

namespace Common.Trackers;

public interface ITracker
{
    Task FetchItems();
    IEnumerable<Item> GetShopItems(int shopId);
    void ClearData();
}