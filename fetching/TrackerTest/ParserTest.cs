using Common.Parsers;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TrackerTest;

[TestClass]
public class NeighborsParserTest
{
    private ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });
    [TestMethod]
    public async Task TestParsing()
    {
        //given
        var cut = ParserFactory.GetInstance().CreateParser("NeighborsParser", _loggerFactory);
        string validString = "";
        using (var fs = new StreamReader("../../../TestData/sosediValidItems.json"))
        {
            validString = await fs.ReadToEndAsync();
        }

        //when
        var itemFields = cut.Parse(validString);

        //then
        var expected = new List<IEnumerable<string>>(){
            new string[]{
                "КОЛБАСА МАДЕРА С/К В/С 1КГ",
                "",
                "",
                "27.59",
                "21.99",
                "/upload/iblock/b93/e2fkm4gnfqxpk5lhgbvix97smtil0nom.jpg"
            },
            new string[]{
                "ВЕТЧИНА БЕЛОВЕЖСКАЯ СВ.К/В В/У 1КГ",
                "Ветчина БЕЛОВЕЖСКАЯ к/в 1 кг Славянский МК",
                "",
                "19.19",
                "12.99",
                "/upload/iblock/682/ekcdrpmnmjrc0jxvdm3en2b2l39d520o.jpg"
            },
            new string[]{
                "КОЛБАСА ВАР.МОРТАД.ПРЕМ.СУС.МАЕН.В/С 1КГ",
                "",
                "",
                "7.49",
                "6.49",
                "/upload/iblock/6c5/7tk0rgqxd5vr5d0crn6ouqy26plte3w3.jpg"
            }
        };
        Assert.AreEqual(expected.Count, itemFields.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            for (var j = 0; j < 6; j++)
            {
                Assert.AreEqual(expected[i].ElementAt(j), itemFields[i].ElementAt(j));
            }
        }
    }

    [TestMethod]
    public async Task TestWrongFormats()
    {
        //given
        var cut = ParserFactory.GetInstance().CreateParser("NeighborsParser", _loggerFactory);
        string invalidString = "";
        using (var fs = new StreamReader("../../../TestData/sosediInvalidItems.json"))
        {
            invalidString = await fs.ReadToEndAsync();
        }

        //when
        try
        {
            var itemFields = cut.Parse(invalidString);
            Assert.Fail();
        }
        catch (JsonException ex)
        {
            //then
            Assert.IsNotNull(ex);
        }
    }
}