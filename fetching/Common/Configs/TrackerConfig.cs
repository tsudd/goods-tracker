namespace Common.Configs;

public class TrackerConfig
{
    public IEnumerable<ScraperConfig> ScrapersConfigurations { get; set; } = new List<ScraperConfig>();
}