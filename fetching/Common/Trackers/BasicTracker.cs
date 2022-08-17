using Common.Configs;
using Common.Parsers;
using Common.Scrapers;
using Microsoft.Extensions.Logging;
using Models;

namespace Common.Trackers;

public class BasicTracker : ITracker
{
    public IEnumerable<IScraper> Scrapers { get; private set; }
    private ILogger<BasicTracker> _logger;
    private TrackerConfig _config;

    public BasicTracker(TrackerConfig config, ILoggerFactory loggerFactory, ScraperFactory scraperFactory, ParserFactory parserFactory)
    {
        _config = config;
        _logger = loggerFactory.CreateLogger<BasicTracker>();
        Scrapers = new List<IScraper>();
        foreach (var conf in _config.ScrapersConfigurations)
        {
            Scrapers.Append<IScraper>(
                scraperFactory.CreateScraper(
                    conf,
                    loggerFactory,
                    parserFactory.CreateParser(conf.ParserName, loggerFactory)));
        }

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