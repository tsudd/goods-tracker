using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Common.Parsers;

public class NeighborsParser : IParser
{
    private ILogger<NeighborsParser> _logger;
    public NeighborsParser(ILogger<NeighborsParser> logger)
    {
        _logger = logger;
    }

    public static string[] PrepareFieldsCollection()
    {
        return new string[]{
            "",
            "",
            "",
            "",
            "",
            ""
        };
    }

    public List<IEnumerable<string>> Parse(string rawItems)
    {
        // throw new NotImplementedException();
        var answer = new List<IEnumerable<string>>();
        var doc = JsonDocument.Parse(rawItems);
        try
        {
            var items = doc.RootElement.EnumerateArray();
            while (items.MoveNext())
            {
                try
                {
                    var itemFields = CollectItemFields(items.Current.EnumerateArray());
                    answer.Add(itemFields);
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogError("Couldn't find item's fields: {0}", ex.Message);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Wrong structure of the JSON with raw items");
            throw new JsonException(ex.Message);
        }
        return answer;
    }

    private string[] CollectItemFields(JsonElement.ArrayEnumerator itemCombo)
    {
        var fields = NeighborsParser.PrepareFieldsCollection();
        while (itemCombo.MoveNext())
        {
            var element = itemCombo.Current;
            if (fields[0].Length == 0)
            {
                fields[0] = element.GetProperty("title").GetString() ?? "";
                fields[3] = element.GetProperty("priceBack").GetString() ?? "";
                fields[4] = element.GetProperty("price").GetString() ?? "";
                fields[5] = element.GetProperty("src").GetString() ?? "";
            }
            else if (fields[1].Length == 0)
            {
                fields[1] = element.GetProperty("title").GetString() ?? "";
            }
            else
            {
                fields[2] = element.GetProperty("title").GetString() ?? "";
            }
        }
        return fields;
    }
}