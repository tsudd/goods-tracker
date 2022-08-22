using Common.Configs;
using Common.Trackers;
using Common.Scrapers;
using Common.Parsers;
using Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using CsvHelper;
using System.Globalization;


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
if (trackerConfig is null || shopIDs is null)
{
    log.LogError("Couldn't define config for the tracker: wrong format of the configuration file");
    return;
}
trackerConfig.ScrapersConfigurations = config
                                            .GetSection("TrackerConfig:ScrapersConfigurations")
                                            .Get<List<ScraperConfig>>();
log.LogInformation("Tracker to be launched: '{0}'. Number of configs for scrapers: '{1}'",
    trackerConfig.TrackerName,
    trackerConfig.ScrapersConfigurations.Count());

//------------------initialization of the tracker with provided config
log.LogInformation("Tracker instance creation...");
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
log.LogInformation("Saving fetched items in csv files...");
foreach (var i in shopIDs)
{
    var items = tracker.GetShopItems(i);
    if (items is null)
    {
        log.LogWarning("No items from '{0}' shop to save...", i);
        continue;
    }
    using (var write = new StreamWriter($"./{i}.csv"))
    {
        using (var csvWriter = new CsvWriter(write, CultureInfo.CurrentCulture))
        {
            csvWriter.WriteHeader<Item>();
            await csvWriter.NextRecordAsync();
            await csvWriter.WriteRecordsAsync(items);
        }
    }
    log.LogInformation(
        "Have written '{0}' items from '{1}' shop.", items.Count().ToString(), i);
}
log.LogInformation("Tracker has ended its work.");