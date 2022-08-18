using Common.Configs;
using Common.Trackers;
using Common.Scrapers;
using Common.Parsers;
using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;


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
var trackerConfig = config.GetSection("TrackerConfig").Get<TrackerConfig>();
if (trackerConfig is null)
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
log.LogInformation("OK.");
