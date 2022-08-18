using Common.Configs;
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
var trackerName = config.GetSection("TrackerConfig").Get<TrackerConfig>();
trackerName.ScrapersConfigurations = config
                                            .GetSection("TrackerConfig:ScrapersConfigurations")
                                            .Get<List<ScraperConfig>>();
log.LogInformation("Tracker to be launched: '{0}'. Number of configs for scrapers: '{1}'",
    trackerName.TrackerName,
    trackerName.ScrapersConfigurations.Count());

