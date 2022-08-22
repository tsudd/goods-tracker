using System.Text.Json;
using Common.Configs;
using Common.Parsers;
using Common.Scrapers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Models;

namespace Common.Trackers;

public class BasicTracker : ITracker
{
    public List<IScraper> Scrapers { get; private set; }
    private List<Tuple<string, List<Item>>> _shopItems;
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
        _shopItems = new List<Tuple<string, List<Item>>>();
        Scrapers = new List<IScraper>();
        _logger.LogInformation("Scrapers creation from provided configs...");
        foreach (var conf in _config.ScrapersConfigurations)
        {
            Scrapers.Add(
                scraperFactory.CreateScraper(
                conf,
                loggerFactory,
                parserFactory.CreateParser(conf.ParserName, loggerFactory)));
            _shopItems.Add(
                new Tuple<string, List<Item>>(conf.ShopID, new List<Item>()));
            _logger.LogInformation("'{0}' was created", conf.Name);
        }
        _logger.LogInformation("Tracker was created.");
    }

    public void ClearData()
    {
        foreach (var shopItems in _shopItems)
        {
            shopItems.Item2.Clear();
        }
    }

    public async Task FetchItems()
    {
        foreach (var scraper in Scrapers)
        {
            var conf = scraper.GetConfig();
            _logger.LogInformation("Scraping from '{0}'...", conf.ShopName);
            try
            {
                _shopItems
                .Where(entry => entry.Item1 == conf.ShopID)
                .Single()
                .Item2.AddRange(await scraper.GetItems());
            }
            catch (JsonException ex)
            {
                NotifyScraperError(conf, ex);
            }
            catch (HtmlWebException ex)
            {
                NotifyScraperError(conf, ex);
            }
            catch (Exception ex)
            {
                NotifyScraperError(conf, ex);
            }
        }
    }

    private void NotifyScraperError(ScraperConfig conf, Exception ex)
    {
        _logger.LogWarning(string.Format("'{0}' has ended its work: {1}", conf.Name, ex.Message));
    }

    public IEnumerable<Item>? GetShopItems(string shopId)
    {
        try
        {
            return _shopItems
                .Where(entry => entry.Item1 == shopId)
                .Single()
                .Item2;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("'{0}' couldn't return shop items: {1}", _config.TrackerName, ex.Message);
            return null;
        }
    }
}