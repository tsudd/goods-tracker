using GoodsTracker.DataCollector.Common.Parsers.Factories;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace TrackerTest;

[TestClass]
public class YaNeighborsParserTest
{
    private ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });
    [TestMethod]
    public async Task TestParsing()
    {
        // given
        var cut = ParserFactory.GetInstance().CreateParser("YaNeighborsParser", _loggerFactory);
        HtmlDocument page = new HtmlDocument();
        using (var fs = new StreamReader("../../../TestData/YaNeighborsItemPage.html"))
        {
            page.LoadHtml(await fs.ReadToEndAsync());
        }

        //when
        var fields = cut.ParseItem(page);

        //then
        Assert.Fail();
    }
}