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
// var factories = config.GetSection("Factories");
// TrackerFactory? trackerFactory = typeof(TrackerFactory);
// ScraperFactory? scraperFactory;
// AdapterFactory? adapterFactory;
log.LogInformation("Tracker to be launched: '{0}'. Number of configs for scrapers: '{1}'",
    trackerConfig.TrackerName,
    trackerConfig.ScrapersConfigurations.Count());

//------------------initialization of the tracker with provided config
// log.LogInformation("Tracker instance creation...");
ITracker? tracker;
try
{
    tracker = TrackerFactory
        .GetInstance()
        .CreateTracker(
            trackerConfig,
            loggerFactory,
            ScraperFactory.GetInstance(),
            ParserFactory.GetInstance());
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
await tracker.FetchItems();

//------------------record fetch data

log.LogInformation("Sending fetched data to HANA db");

try
{
    var adapter = AdapterFactory.GetInstance().CreateAdapter(adapterConfig, loggerFactory);
    adapter.SaveItems(tracker, shopIDs);
}
catch (ApplicationException ex)
{
    log.LogWarning($"Error occured during saving of items: {ex.Message}. Saving items locally for future restore");
    //local saving code...
}

//------------------clearing & disposing
log.LogInformation("Clearing fetched data...");
tracker.ClearData();

log.LogInformation("Tracker has ended its work.");