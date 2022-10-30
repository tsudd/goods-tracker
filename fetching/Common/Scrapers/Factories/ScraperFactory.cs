using Common.Configs;
using Common.Mapers.Interfaces;
using Common.Parsers.Interfaces;
using Common.Requesters;
using Common.Scrapers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Common.Scrapers.Factories;

public partial class ScraperFactory
{
    protected static ScraperFactory? _instance;
    private ScraperFactory()
    {
    }

    public static ScraperFactory GetInstance()
    {
        if (_instance is null)
        {
            _instance = new ScraperFactory();
        }
        return _instance;
    }

    public IScraper CreateScraper(
        ScraperConfig scraperConfig,
        ILoggerFactory loggerFactory,
        IItemParser parser,
        IItemMapper? mapper = null,
        IRequester? requester = null)
    {
        switch (scraperConfig.Name)
        {
            case nameof(YaNeighborsScraper):
                return new YaNeighborsScraper(
                    scraperConfig,
                    loggerFactory.CreateLogger<YaNeighborsScraper>(),
                    parser,
                    mapper,
                    requester);
            default:
                throw new ArgumentException(
                    $"couldn't create {scraperConfig.Name}: no such scraper in the app");
        }
    }

    public static ScraperFactory GetSpecifiedFactory(string typeName)
    {
        try
        {
            return (ScraperFactory)(Type.GetType(typeName)?.GetMethod("GetInstance")?.Invoke(null, null)
            ?? throw new ArgumentException("No such scraper factory in the app"));
        }
        catch (ArgumentException ex)
        {
            throw ex;
        }
    }
}