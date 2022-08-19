using Common.Configs;
using Common.Parsers;
using Common.Scrapers;
using Common.Requesters;
using Models;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Diagnostics.CodeAnalysis;

namespace TrackerTest;


public class NeighborsParserMock : IParser
{
    public NeighborsParserMock()
    {
        ResultItems = new List<Item>(){
            new Item(){
                Name1 = "КОЛБАСА МАДЕРА С/К В/С 1КГ",
                Price = "21.99",
                Discount = "27.59",
                Link = "/upload/iblock/b93/e2fkm4gnfqxpk5lhgbvix97smtil0nom.jpg"
            },
            new Item(){
                Name1 = "ВЕТЧИНА БЕЛОВЕЖСКАЯ СВ.К/В В/У 1КГ",
                Name2 = "Ветчина БЕЛОВЕЖСКАЯ к/в 1 кг Славянский МК",
                Price = "12.99",
                Discount = "19.19",
                Link = "/upload/iblock/682/ekcdrpmnmjrc0jxvdm3en2b2l39d520o.jpg"
            },
            new Item(){
                Name1 = "КОЛБАСА ВАР.МОРТАД.ПРЕМ.СУС.МАЕН.В/С 1КГ",
                Price = "6.49",
                Discount = "7.49",
                Link = "/upload/iblock/6c5/7tk0rgqxd5vr5d0crn6ouqy26plte3w3.jpg"
            },
            new Item(){
                Name1 = "КОЛБАСА МАДЕРА С/К В/С 1КГ",
                Price = "21.99",
                Discount = "27.59",
                Link = "/upload/iblock/b93/e2fkm4gnfqxpk5lhgbvix97smtil0nom.jpg"
            },
            new Item(){
                Name1 = "ВЕТЧИНА БЕЛОВЕЖСКАЯ СВ.К/В В/У 1КГ",
                Name2 = "Ветчина БЕЛОВЕЖСКАЯ к/в 1 кг Славянский МК",
                Price = "12.99",
                Discount = "19.19",
                Link = "/upload/iblock/682/ekcdrpmnmjrc0jxvdm3en2b2l39d520o.jpg"
            },
            new Item(){
                Name1 = "КОЛБАСА ВАР.МОРТАД.ПРЕМ.СУС.МАЕН.В/С 1КГ",
                Price = "6.49",
                Discount = "7.49",
                Link = "/upload/iblock/6c5/7tk0rgqxd5vr5d0crn6ouqy26plte3w3.jpg"
            },
        };
    }

    public static List<Item> ResultItems { get; private set; } = new List<Item>();
    public List<IEnumerable<string>> Parse(string rawItems)
    {
        var answer = new List<IEnumerable<string>>();
        answer.Add(new string[]{
            NeighborsParserMock.ResultItems[0].Name1,
            NeighborsParserMock.ResultItems[0].Name2,
            NeighborsParserMock.ResultItems[0].Name3,
            NeighborsParserMock.ResultItems[0].Price,
            NeighborsParserMock.ResultItems[0].Discount,
            NeighborsParserMock.ResultItems[0].Link,
        });
        answer.Add(new string[]{
            NeighborsParserMock.ResultItems[1].Name1,
            NeighborsParserMock.ResultItems[1].Name2,
            NeighborsParserMock.ResultItems[1].Name3,
            NeighborsParserMock.ResultItems[1].Price,
            NeighborsParserMock.ResultItems[1].Discount,
            NeighborsParserMock.ResultItems[1].Link,
        });
        answer.Add(new string[]{
            NeighborsParserMock.ResultItems[2].Name1,
            NeighborsParserMock.ResultItems[2].Name2,
            NeighborsParserMock.ResultItems[2].Name3,
            NeighborsParserMock.ResultItems[2].Price,
            NeighborsParserMock.ResultItems[2].Discount,
            NeighborsParserMock.ResultItems[2].Link,
        });
        return answer;
    }
}

public class NeighborsRequesterGoodMock : IRequester
{
    private string _message1;
    private string _message2;
    private bool wasRequested = false;
    public NeighborsRequesterGoodMock()
    {
        using (var fs = new StreamReader("../../../TestData/sosediResponse1.json", true))
        {
            _message1 = fs.ReadToEnd();
        }
        using (var fs = new StreamReader("../../..//TestData/sosediResponse1.json", true))
        {
            _message2 = fs.ReadToEnd();
        }
    }

    public Task<string> GetAsync(string url)
    {
        throw new NotImplementedException();
    }

    public Task<HtmlDocument> GetPageHtmlAsync(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<string> PostAsync(string url, Dictionary<string, string>? headers = null, string data = "")
    {
        await Task.Delay(0);
        if (wasRequested)
        {
            return _message2;
        }
        else
        {
            wasRequested = !wasRequested;
            return _message1;
        }
    }
}

public class NeighborsRequesterBadMock : IRequester
{
    public Task<string> GetAsync(string url)
    {
        throw new NotImplementedException();
    }

    public Task<HtmlDocument> GetPageHtmlAsync(string url)
    {
        throw new NotImplementedException();
    }

    public Task<string> PostAsync(string url, Dictionary<string, string>? headers = null, string data = "")
    {
        throw new HtmlWebException("bad request");
    }
}

[TestClass]
public class NeighborsScraperTest
{
    private ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });
    public ScraperConfig Config { get; private set; } = new ScraperConfig()
    {
        ShopID = "1",
        Name = "NeighborsScraper",
        ShopName = "Sosedi",
        ParserName = "NeighborsParser",
        ShopUrl = "https://sosedi.by/",
        ShopApi = "https://sosedi.by/local/api/getListProducts.php"
    };
    public NeighborsScraperTest()
    {
    }

    [TestMethod]
    public async Task TestItemsReturn()
    {
        //given
        var cut = ScraperFactory
            .GetInstance()
            .CreateScraper(
                Config,
                _loggerFactory,
                new NeighborsParserMock(),
                new NeighborsRequesterGoodMock());

        //when
        var items = await cut.GetItems();

        //then
        var ind = 0;
        foreach (var i in items)
        {
            Assert.IsTrue(i.Equals(NeighborsParserMock.ResultItems.ElementAt(ind)));
            ind++;
        }
    }

    [TestMethod]
    public async Task TestBaseItemsReturn()
    {
        //given
        var cut = ScraperFactory
            .GetInstance()
            .CreateScraper(
                Config,
                _loggerFactory,
                new NeighborsParserMock(),
                new NeighborsRequesterBadMock());

        //when
        try
        {
            var items = await cut.GetItems();
            Assert.Fail();
        }
        catch (HtmlWebException ex)
        {
            Assert.IsNotNull(ex);
        }
    }

}