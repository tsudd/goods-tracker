using Common.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Common.Trackers.Factories;
using Common.Scrapers.Factories;
using Common.Parsers.Factories;
using Common.Trackers.Interfaces;
using Common.Adapters.Factories;
using Common.Mapers.Factories;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{environment}.json")
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
log.LogInformation($"Configuration was loaded. Tracker is starting now in {environment} mode.");

//------------------handling tracker configuration
// TODO: get tracker configuraiton deeply 
var trackerConfig = new TrackerConfig()
{
    TrackerName = config.GetSection("TrackerConfig").GetValue<string>("TrackerName"),
    ScrapersConfigurations = config.GetSection("TrackerConfig:ScrapersConfigurations").Get<List<ScraperConfig>>()
};
var shopIDs = config.GetSection("ShopIDs").Get<IEnumerable<string>>();
var adapterConfig = config.GetSection("AdapterConfig").Get<AdapterConfig>();
var alternativeAdapterConfig = config.GetSection("AlternativeAdapterConfig").Get<AdapterConfig>();
if (trackerConfig is null || shopIDs is null || adapterConfig is null)
{
    log.LogError("Couldn't define config for the tracker: wrong format of the configuration file");
    return;
}
if (alternativeAdapterConfig is null)
{
    log.LogWarning("Couldn't get config for alternative data adapter. Data might be lost by proceeding without it.");
}
var factories = config.GetSection("Factories");
TrackerFactory? trackerFactory;
ScraperFactory? scraperFactory;
AdapterFactory? adapterFactory;
ParserFactory? parserFactory;
MapperFactory? mapperFactory;
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
    mapperFactory = MapperFactory.GetSpecifiedFactory(
        factories.GetSection(typeof(MapperFactory).Name).Get<string>());
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
IItemTracker? tracker;
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
await tracker.FetchItems();

//------------------record fetch data

log.LogInformation("Sending fetched data to the DB adapter");

try
{
    var adapter = adapterFactory.CreateAdapter(adapterConfig, loggerFactory);
    adapter.SaveItems(tracker, shopIDs);
}
catch (ApplicationException ex)
{
    if (alternativeAdapterConfig is not null)
    {
        log.LogWarning(
        $"Error occured during saving of items into the DB: {ex.Message}. Saving items using alternative data adapter for future restore");
        var alternativeAdapter = adapterFactory.CreateAdapter(alternativeAdapterConfig, loggerFactory);
        alternativeAdapter.SaveItems(tracker, shopIDs);
    }
    else
    {
        log.LogError($"Error occured during data save: {ex.Message}");
    }

}

//------------------clearing & disposing
log.LogInformation("Clearing fetched data...");
tracker.ClearData();

log.LogInformation("Tracker has ended its work.");