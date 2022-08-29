using Common.Configs;
using Common.Requesters;
using Common.Parsers;
using Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;

namespace Common.Scrapers;

public class NeighborsScraper : IScraper
{
    public IRequester Requester { get; private set; }
    private ILogger<NeighborsScraper> _logger;
    private ScraperConfig _config;
    private IParser _parser;
    public NeighborsScraper(ScraperConfig config, ILogger<NeighborsScraper> logger, IParser parser, IRequester? requester = null)
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

    public async Task<IEnumerable<Item>> GetItems()
    {
        var page = 1;
        var pagesAmount = 0;
        var items = new List<Item>();
        do
        {
            var body = BuildRequestDataForPage(page);
            try
            {
                var jsonData = await RequestJSONDataWithBody(body);
                pagesAmount = (pagesAmount == 0) ?
                (jsonData.RootElement.GetProperty("pages").GetInt32()) :
                (pagesAmount - 1);
                items.AddRange(ParseItemsFromJSON(jsonData.RootElement));
            }
            catch (JsonException ex)
            {
                var message = $"wrong JSON structure: {ex.Message}";
                _logger.LogError(message);
                throw new FormatException(message);
            }
            catch (HtmlWebException ex)
            {
                var message = $"error while scraping data: {ex.Message}";
                _logger.LogError(message);
                throw new HtmlWebException(message);
            }
            page++;
        } while (pagesAmount != 1);
        _logger.LogInformation("'{0}' items where scrapped from '{1}'", items.Count, _config.ShopName);
        return items;
    }

    private string BuildRequestDataForPage(int pageNumber)
    {
        return "{{\"selected\": \"all\", " +
            $"\"selectedCategory\": \"all\", \"paginationItem\": {pageNumber.ToString()}}}";
    }

    private async Task<JsonDocument> RequestJSONDataWithBody(string requestBody)
    {
        return JsonDocument.Parse(await Requester.PostAsync(_config.ShopApi, null, requestBody));
    }

    private IEnumerable<Item> ParseItemsFromJSON(JsonElement root)
    {
        var itemsFields = _parser.Parse(root.GetProperty("items").GetRawText());
        var itemsPortion = new List<Item>();
        foreach (var fields in itemsFields)
        {
            //TODO: out of range exception
            itemsPortion.Add(
                new Item()
                {
                    Name1 = fields.ElementAt<string>(0),
                    Name2 = fields.ElementAt<string>(1),
                    Name3 = fields.ElementAt<string>(2),
                    Price = fields.ElementAt<string>(3),
                    Discount = fields.ElementAt<string>(4),
                    Link = fields.ElementAt<string>(5)
                }
            );
        }
        return itemsPortion;
    }

    public ScraperConfig GetConfig()
    {
        return new ScraperConfig()
        {
            Name = _config.Name,
            ParserName = _config.ParserName,
            ShopID = _config.ShopID,
            ShopUrl = _config.ShopUrl,
            ShopApi = _config.ShopApi,
            ShopName = _config.ShopName
        };
    }
}