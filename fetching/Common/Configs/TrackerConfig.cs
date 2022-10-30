namespace Common.Configs;

public class TrackerConfig
{
    public string TrackerName { get; init; } = String.Empty;
    public IEnumerable<ScraperConfig> ScrapersConfigurations { get; init; } = new List<ScraperConfig>();
}