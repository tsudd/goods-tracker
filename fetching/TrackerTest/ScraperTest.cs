using GoodsTracker.DataCollector.Common.Configs;
using GoodsTracker.DataCollector.Common.Parsers;
using GoodsTracker.DataCollector.Common.Requesters;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using GoodsTracker.DataCollector.Common.Scrapers.Factories;

namespace TrackerTest;

[TestClass]
public class YaNeighborsScraperTest
{
    private ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });
    public class YaNeighborsRequesterMock : IRequester
    {
        public async Task<string> GetAsync(string url)
        {
            if (url == "https://eda.yandex.by/retail/sosedi_supermarket?placeSlug=sosedi_keyaq")
            {
                using (var fs = new StreamReader("../../../TestData/YaNeighborsCatalog.html", true))
                {
                    return await fs.ReadToEndAsync();
                }
            }
            else if (url == "https://eda.yandex.by/retail/sosedi_supermarket/catalog/165?placeSlug=sosedi_keyaq")
            {
                using (var fs = new StreamReader("../../../TestData/YaNeighborsCategory.html", true))
                {
                    return await fs.ReadToEndAsync();
                }
            }
            else if (url == "https://eda.yandex.by/retail/sosedi_supermarket/product/46bcac61-a8cb-4b85-a3f5-3d866b7e1f42?placeSlug=sosedi_keyaq")
            {
                using (var fs = new StreamReader("../../../TestData/YaNeighborsItemPage.html", true))
                {
                    return await fs.ReadToEndAsync();
                }
            }
            throw new HtmlWebException("bad request");
        }

        public Task<HtmlDocument> GetPageHtmlAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostAsync(string url, Dictionary<string, string>? headers = null, string data = "")
        {
            throw new NotImplementedException();
        }
    }
    public ScraperConfig Config { get; private set; } = new ScraperConfig()
    {
        ShopID = "1",
        Name = "YaNeighborsScraper",
        ShopName = "Sosedi",
        ParserName = "NeighborsParser",
        ShopUrl = "https://eda.yandex.by",
        ShopStartRecource = "/retail/sosedi_supermarket?placeSlug=sosedi_keyaq",
        ShopApi = "https://sosedi.by/local/api/getListProducts.php"
    };

    [TestMethod]
    public async Task TestYaNeighborsScraper()
    {
        //given
        var cut = ScraperFactory
                    .GetInstance()
                    .CreateScraper(
                        Config,
                        _loggerFactory,
                        new YaNeighborsParser(_loggerFactory.CreateLogger<YaNeighborsParser>()),
                        null,
                        new YaNeighborsRequesterMock()
                    );
        //when
        var items = await cut.GetItems();

        //then
        Assert.Fail();

    }

}