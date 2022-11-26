using GoodsTracker.DataCollector.Common.Adapters.Interfaces;
using GoodsTracker.DataCollector.Common.Configs;
using GoodsTracker.DataCollector.Common.Mappers.Interfaces;
using GoodsTracker.DataCollector.Common.Parsers.Interfaces;
using GoodsTracker.DataCollector.Common.Requesters.Interfaces;
using GoodsTracker.DataCollector.Common.Scrapers.Interfaces;
using GoodsTracker.DataCollector.Common.Trackers.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoodsTracker.DataCollector.Common.Factories.Interfaces;
public interface IDataCollectorFactory : IDisposable
{
    IItemMapper CreateMapper(string mapperName);
    IItemParser CreateParser(string parserName, ILoggerFactory loggerFactory);
    IScraper CreateScraper(
        ScraperConfig config,
        ILoggerFactory loggerFactory,
        IItemParser? parser = null,
        IItemMapper? mapper = null,
        IRequester? requester = null
    );
    IItemTracker CreateTracker(
        TrackerConfig config,
        ILoggerFactory loggerFactory);
    IRequester CreateRequester(string requesterName, HttpClient? client = null);
    IDataAdapter CreateDataAdapter(AdapterConfig config, ILoggerFactory loggerFactory);
}