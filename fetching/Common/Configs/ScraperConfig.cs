namespace Common.Configs;

public class ScraperConfig
{
    public string Name { get; set; } = String.Empty;
    public string ParserName { get; set; } = String.Empty;
    public string ShopName { get; set; } = String.Empty;
    public string ShopID { get; set; } = String.Empty;
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public string ShopUrl { get; set; } = String.Empty;
    public string ShopStartRecource { get; set; } = String.Empty;
    public string ShopApi { get; set; } = String.Empty;
    public Dictionary<string, string> HTMLSections { get; set; } = new Dictionary<string, string>();
}