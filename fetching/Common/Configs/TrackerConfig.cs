namespace Common.Configs;

public class TrackerConfig
{
    public string TrackerName { get; set; } = String.Empty;
    public IEnumerable<ScraperConfig> ScrapersConfigurations { get; set; } = new List<ScraperConfig>();
}