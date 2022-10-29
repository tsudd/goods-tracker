using HtmlAgilityPack;
using Models.Constants;

namespace Common.Parsers.Interfaces;

public interface IItemParser
{
    List<Dictionary<string, string>> ParseItems(string rawItems);
    Dictionary<string, string> ParseItem(string rawItem);
    Dictionary<ItemFields, string> ParseItem(HtmlDocument itemPage);
}