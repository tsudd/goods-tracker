namespace Common.Configs;

public class ScraperConfig
{
    public string Name { get; init; } = String.Empty;
    public string ParserName { get; init; } = String.Empty;
    public string ShopName { get; init; } = String.Empty;
    public string ShopID { get; init; } = String.Empty;
    public Dictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public string ShopUrl { get; init; } = String.Empty;
    public string ShopStartRecource { get; init; } = String.Empty;
    public string ShopApi { get; init; } = String.Empty;
    public Dictionary<string, string> HTMLSections { get; init; } = new Dictionary<string, string>();
}