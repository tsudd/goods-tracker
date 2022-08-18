using Common.Configs;
using Common.Parsers;
using Common.Scrapers;
using Microsoft.Extensions.Logging;
using Models;

namespace Common.Trackers;

public class BasicTracker : ITracker
{
    public List<IScraper> Scrapers { get; private set; }
    private ILogger<BasicTracker> _logger;
    private TrackerConfig _config;

    public BasicTracker(
        TrackerConfig config,
        ILoggerFactory loggerFactory,
        ScraperFactory scraperFactory,
        ParserFactory parserFactory)
    {
        _config = config;
        _logger = loggerFactory.CreateLogger<BasicTracker>();
        Scrapers = new List<IScraper>();
        _logger.LogInformation("Scrapers creation from provided configs...");
        foreach (var conf in _config.ScrapersConfigurations)
        {
            _logger.LogInformation("Creating scraper: '{0}'", conf.Name);
            Scrapers.Add(
                scraperFactory.CreateScraper(
                conf,
                loggerFactory,
                parserFactory.CreateParser(conf.ParserName, loggerFactory)));
        }
        _logger.LogInformation("Tracker was created.");
    }

    public void ClearData()
    {
        throw new NotImplementedException();
    }

    public Task FetchItems()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Item> GetShopItems(int shopId)
    {
        throw new NotImplementedException();
    }
}