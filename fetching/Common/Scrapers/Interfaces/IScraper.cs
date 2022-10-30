using GoodsTracker.DataCollector.Models;
using GoodsTracker.DataCollector.Common.Configs;

namespace GoodsTracker.DataCollector.Common.Scrapers.Interfaces;

public interface IScraper
{
    Task<IEnumerable<Item>> GetItems();
    ScraperConfig GetConfig();
}