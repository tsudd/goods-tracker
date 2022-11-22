using GoodsTracker.DataCollector.Common.Adapters.Implementations;
using GoodsTracker.DataCollector.Common.Adapters.Interfaces;
using GoodsTracker.DataCollector.Common.Configs;
using GoodsTracker.DataCollector.Common.Factories.Interfaces;
using GoodsTracker.DataCollector.Common.Mappers.Implementations;
using GoodsTracker.DataCollector.Common.Mappers.Interfaces;
using GoodsTracker.DataCollector.Common.Parsers.Implementations;
using GoodsTracker.DataCollector.Common.Parsers.Interfaces;
using GoodsTracker.DataCollector.Common.Requesters.Implementaions;
using GoodsTracker.DataCollector.Common.Requesters.Interfaces;
using GoodsTracker.DataCollector.Common.Scrapers.Implementaions;
using GoodsTracker.DataCollector.Common.Scrapers.Interfaces;
using GoodsTracker.DataCollector.Common.Trackers.Implementations;
using GoodsTracker.DataCollector.Common.Trackers.Interfaces;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GoodsTracker.DataCollector.Common.Factories.Implementations;
public class DataCollectorFactory : IDataCollectorFactory
{
    protected static IDataCollectorFactory? _instance;
    protected static IWebDriver? _driverInstanse;
    protected DataCollectorFactory() { }

    public IDataAdapter CreateDataAdapter(AdapterConfig config, ILoggerFactory loggerFactory)
    {
        return config.AdapterName switch
        {
            nameof(HanaAdapter) => new HanaAdapter(
                    config,
                    loggerFactory.CreateLogger<HanaAdapter>()),
            nameof(CsvAdapter) => new CsvAdapter(
                    config,
                    loggerFactory.CreateLogger<CsvAdapter>()),
            var _ =>
                throw new ArgumentException(
                    $"couldn't create {config.AdapterName}: no such data adapter in the app"
                )
        };
    }

    public IItemMapper CreateMapper(string mapperName)
    {
        return mapperName switch
        {
            nameof(BasicMapper) => new BasicMapper(),
            var _ => throw new ArgumentException($"couldn't create {mapperName}: no such mapper in the app."),
        };
    }

    public IItemParser CreateParser(
        string parserName,
        ILoggerFactory loggerFactory)
    {
        return parserName switch
        {
            nameof(YaNeighborsParser) => new YaNeighborsParser(loggerFactory.CreateLogger<YaNeighborsParser>()),
            nameof(EmallParser) => new EmallParser(loggerFactory.CreateLogger<EmallParser>()),
            var _ => throw new ArgumentException($"couldn't create {parserName}: no such parser in the app."),
        };
    }

    public IRequester CreateRequester(string requesterName, HttpClient? client = null)
    {
        return requesterName switch
        {
            nameof(BasicRequester) => new BasicRequester(client),
            var _ => throw new ArgumentException($"coudln't create {requesterName}")
        };
    }

    public IScraper CreateScraper(
        ScraperConfig config,
        ILoggerFactory loggerFactory,
        IItemParser? parser = null,
        IItemMapper? mapper = null,
        IRequester? requester = null)
    {
        return config.Name switch
        {
            nameof(YaNeighborsScraper) => new YaNeighborsScraper(
                    config,
                    loggerFactory.CreateLogger<YaNeighborsScraper>(),
                    parser ?? CreateParser(config.ParserName, loggerFactory),
                    mapper,
                    requester),
            nameof(EmallScraper) => new EmallScraper(
                config,
                loggerFactory.CreateLogger<EmallScraper>(),
                parser ?? CreateParser(config.ParserName, loggerFactory),
                mapper ?? CreateMapper(config.ItemMapper),
                GetWebDriverInstance()
            ),
            var _ =>
                    throw new ArgumentException(
                        $"couldn't create {config.Name}: no such scraper in the app"),
        };
    }

    public IItemTracker CreateTracker(
        TrackerConfig config,
        ILoggerFactory loggerFactory)
    {
        return config.TrackerName switch
        {
            nameof(BasicTracker) =>
                new BasicTracker(
                    config,
                    loggerFactory,
                    this
                ),
            var _ =>
                throw new ArgumentException(
                    $"couldn't create {config.TrackerName}: no such tracker in the app"),
        };
    }

    public static IDataCollectorFactory GetInstance()
    {
        if (_instance is null)
        {
            _instance = new DataCollectorFactory();
        }
        return _instance;
    }

    protected static IWebDriver GetWebDriverInstance()
    {
        if (_driverInstanse is null)
        {
            _driverInstanse = new ChromeDriver();
        }
        return _driverInstanse;
    }
}