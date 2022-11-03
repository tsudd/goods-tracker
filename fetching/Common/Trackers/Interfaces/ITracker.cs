using GoodsTracker.DataCollector.Models;
using GoodsTracker.DataCollector.Common.Configs;

namespace GoodsTracker.DataCollector.Common.Trackers.Interfaces;

public interface IItemTracker
{
    Task FetchItemsAsync();
    IEnumerable<Item>? GetShopItems(string shopId);
    void ClearData();
}