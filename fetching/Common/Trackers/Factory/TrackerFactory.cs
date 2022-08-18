using Common.Configs;
using Microsoft.Extensions.Logging;
using Common.Scrapers;
using Common.Parsers;

namespace Common.Trackers;

public class TrackerFactory
{
    private static TrackerFactory? _instance;

    private TrackerFactory()
    {

    }

    public static TrackerFactory GetInstance()
    {
        if (_instance is null)
        {
            _instance = new TrackerFactory();
        }
        return _instance;
    }

    public ITracker CreateTracker(
        TrackerConfig config,
        ILoggerFactory loggerFactory,
        ScraperFactory scraperFactory,
        ParserFactory parserFactory)
    {
        switch (config.TrackerName)
        {
            case nameof(BasicTracker):
                return new BasicTracker(
                    config,
                    loggerFactory,
                    scraperFactory,
                    parserFactory
                );
            default:
                throw new ArgumentException(
                    string.Format("couldn't create {0}: no such tracker in the app", config.TrackerName));
        }
    }
}