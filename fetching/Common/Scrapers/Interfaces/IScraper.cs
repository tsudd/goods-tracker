using Models;
using Common.Configs;

namespace Common.Scrapers.Interfaces;

public interface IScraper
{
    Task<IEnumerable<Item>> GetItems();
    ScraperConfig GetConfig();
}