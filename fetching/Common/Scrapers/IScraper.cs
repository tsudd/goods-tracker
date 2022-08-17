using Models;
using Common.Configs;

namespace Common.Scrapers;

public interface IScraper
{
    Task<IEnumerable<Item>> GetItems();
    string GetConfig();
}