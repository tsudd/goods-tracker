using Common.Configs;
using Common.Requesters;
using Common.Parsers;
using Models;
using Microsoft.Extensions.Logging;

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
    }

    public Task<IEnumerable<Item>> GetItems()
    {
        throw new NotImplementedException();
    }

    public string GetConfig()
    {
        throw new NotImplementedException();
    }
}