using Models;
using Common.Configs;

namespace Common.Trackers.Interfaces;

public interface IItemTracker
{
    Task FetchItems();
    IEnumerable<Item>? GetShopItems(string shopId);
    void ClearData();
}