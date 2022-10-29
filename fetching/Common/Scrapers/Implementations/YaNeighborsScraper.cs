using Common.Configs;
using Microsoft.Extensions.Logging;
using Models;
using Common.Requesters;
using HtmlAgilityPack;
using Common.Parsers.Interfaces;
using Common.Scrapers.Interfaces;

namespace Common.Scrapers;
public sealed class YaNeighborsScraper : IScraper
{
    public IRequester Requester { get; private set; }
    private IItemParser _parser;
    private ScraperConfig _config;
    private ILogger<YaNeighborsScraper> _logger;

    public YaNeighborsScraper(
        ScraperConfig config,
        ILogger<YaNeighborsScraper> logger,
        IItemParser parser,
        IRequester? requester = null)
    {
        if (requester is null)
        {
            Requester = new BasicRequester();
        }
        else
        {
            Requester = requester;
        }
        _logger = logger;
        _config = config;
        _parser = parser;
        _logger.LogInformation("Scraper was created");
    }

    public ScraperConfig GetConfig()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Item>> GetItems()
    {
        var categories = await GetCategoryLinksAsync();
        var items = new List<Item>();

        foreach (var category in categories)
        {
            items.AddRange(await ProcessCategoryPageAsync(category.CategoryLink));
        }

        return items;
    }

    private async Task<IEnumerable<(string CategoryLink, string CategoryName)>> GetCategoryLinksAsync()
    {
        var links = new List<(string CategoryLink, string CategoryName)>();

        var htmlDoc = await GetHtmlDocumentAsync(_config.ShopUrl + _config.ShopStartRecource);
        var rawLinks =
            htmlDoc
                .DocumentNode
                .SelectNodes("//div[@class='UiKitShopMenu_root']/ul/li/a");

        foreach (var raw in rawLinks)
        {
            links.Add((
                raw.Attributes["href"].Value,
                raw.SelectSingleNode("div[@class='UiKitDesktopShopMenuItem_text']").InnerText));
        }

        return links;
    }

    private async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(await Requester.GetAsync(url));
        return doc;
    }

    private async Task<IEnumerable<Item>> ProcessCategoryPageAsync(string recourse)
    {
        var page = await GetHtmlDocumentAsync(_config.ShopUrl + recourse);
        var itemRecourses = new List<string>();

        var rawLinks =
            page
                .DocumentNode
                .SelectNodes("//div[@class='DesktopGoodsList_root']/ul/li/a");
        if (rawLinks is not null)
        {
            foreach (var raw in rawLinks)
            {
                itemRecourses.Add(raw.Attributes["href"].Value);
            }
        }

        var parsedItems = new List<Item>();
        foreach (var itemRecourse in itemRecourses)
        {
            var itemPage = await GetHtmlDocumentAsync(_config.ShopUrl + itemRecourse);
            var itemFields = _parser.ParseItem(itemPage);
            parsedItems.Add(_parser.)
        }

        return parsedItems;
    }
}