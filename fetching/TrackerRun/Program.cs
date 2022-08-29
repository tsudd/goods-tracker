using Common.Configs;
using Common.Trackers;
using Common.Scrapers;
using Common.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Common.Adapters;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Information)
        .AddConsole()
        .AddConfiguration(config.GetSection("Logging"));
});
var log = loggerFactory.CreateLogger<Program>();
log.LogInformation("Configuration was loaded. Tracker is starting now.");

//------------------handling tracker configuration
var trackerConfig = config.GetSection("TrackerConfig").Get<TrackerConfig>();
var shopIDs = config.GetSection("ShopIDs").Get<IEnumerable<string>>();
var adapterConfig = config.GetSection("AdapterConfig").Get<AdapterConfig>();
if (trackerConfig is null || shopIDs is null || adapterConfig is null)
{
    log.LogError("Couldn't define config for the tracker: wrong format of the configuration file");
    return;
}
trackerConfig.ScrapersConfigurations = config
                                            .GetSection("TrackerConfig:ScrapersConfigurations")
                                            .Get<List<ScraperConfig>>();
var factories = config.GetSection("Factories");
TrackerFactory? trackerFactory;
ScraperFactory? scraperFactory;
AdapterFactory? adapterFactory;
ParserFactory? parserFactory;
try
{
    trackerFactory = TrackerFactory.GetSpecifiedFactory(
        factories.GetSection(typeof(TrackerFactory).Name).Get<string>());
    scraperFactory = ScraperFactory.GetSpecifiedFactory(
        factories.GetSection(typeof(ScraperFactory).Name).Get<string>());
    adapterFactory = AdapterFactory.GetSpecifiedFactory(
        factories.GetSection(typeof(AdapterFactory).Name).Get<string>());
    parserFactory = ParserFactory.GetSpecifiedFactory(
        factories.GetSection(typeof(ParserFactory).Name).Get<string>());
}
catch (ArgumentException ex)
{
    log.LogError($"Invalid configuration for factories: {ex.Message}");
    return;
}
log.LogInformation("Tracker to be launched: '{0}'. Number of configs for scrapers: '{1}'",
    trackerConfig.TrackerName,
    trackerConfig.ScrapersConfigurations.Count());

//------------------initialization of the tracker with provided config
log.LogInformation("Tracker instance creation...");
ITracker? tracker;
try
{
    tracker = trackerFactory
        .CreateTracker(
            trackerConfig,
            loggerFactory,
            scraperFactory,
            parserFactory
            );
}
catch (ArgumentException ex)
{
    log.LogError("Error while tracker initialization: {0}", ex.Message);
    return;
}
catch (Exception ex)
{
    log.LogError("Unspecified error occurred while tracker creation: {0}", ex.Message);
    return;
}

//------------------fetching data with configured tracker
log.LogInformation("Starting scraping items.");
// await tracker.FetchItems();

//------------------record fetch data

log.LogInformation("Sending fetched data to HANA db");

try
{
    var adapter = adapterFactory.CreateAdapter(adapterConfig, loggerFactory);
    adapter.SaveItems(tracker, shopIDs);
}
catch (ApplicationException ex)
{
    log.LogWarning($"Error occured during saving of items: {ex.Message}. Saving items locally for future restore");
    //local saving code...
}

//------------------clearing & disposing
log.LogInformation("Clearing fetched data...");
// tracker.ClearData();

log.LogInformation("Tracker has ended its work.");