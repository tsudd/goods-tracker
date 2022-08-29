using Common.Configs;
using Common.Parsers;
using Common.Requesters;
using Microsoft.Extensions.Logging;

namespace Common.Scrapers;

public class ScraperFactory
{
    private static ScraperFactory? _instance;
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
        IParser parser,
        IRequester? requester = null)
    {
        switch (scraperConfig.Name)
        {
            case nameof(NeighborsScraper):
                return new NeighborsScraper(
                    scraperConfig,
                    loggerFactory.CreateLogger<NeighborsScraper>(),
                    parser,
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