using GoodsTracker.DataCollector.Common.Parsers.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using GoodsTracker.DataCollector.Models.Constants;
using System.Text.RegularExpressions;
using GoodsTracker.DataCollector.Common.Mappers.Interfaces;
using System.Text;

namespace GoodsTracker.DataCollector.Common.Parsers.Implementations;
public sealed class EmallParser : IItemParser
{
    private const short DESCRIPTIONS_AMOUNT = 4;
    private const string STANDART_PORTION_IN_GRAMMS = "100";
    private readonly static Regex _itemTitleRegex = new Regex(
        @"^(.*[^,])(?:,)?\s((\d+\.?\d*\s?)(\w*)?)$", RegexOptions.Compiled
    );
    private ILogger<EmallParser> _logger;

    public EmallParser(ILogger<EmallParser> logger)
    {
        _logger = logger;
    }

    public Dictionary<string, string> ParseItem(string rawItem)
    {
        throw new NotImplementedException();
    }

    public Dictionary<ItemFields, string> ParseItem(HtmlDocument itemPage)
    {
        // TODO: parse each part of the item in the extension methods for the fields dict
        var fields = new Dictionary<ItemFields, string>();

        var rawTitle = SelectFieldWithNodePath(
            itemPage,
            "//h2"
        )
        ??
        throw new InvalidDataException("Parser couldn't recognize page structure (title)");

        var itemTitleMatch = _itemTitleRegex.Match(rawTitle);
        if (itemTitleMatch.Success)
        {
            fields.Add(
                ItemFields.Name1,
                itemTitleMatch.Groups[1].Value
            );
            fields.Add(
                ItemFields.Weight,
                itemTitleMatch.Groups[3].Value
            );
            fields.Add(
                ItemFields.WeightUnit,
                itemTitleMatch.Groups[4].Value
            );
        }
        else
        {
            fields.Add(
                ItemFields.Name1,
                rawTitle
            );
        }

        var price =
            itemPage
                .DocumentNode
                .SelectSingleNode("//div[@class='price']/meta[@itemprop='price']")
                ?.GetAttributeValue("content", null);
        fields.Add(
            ItemFields.Price,
            price ?? throw new InvalidDataException("Parser couldn't recognize page structure (price)")
        );

        var descriptions =
            itemPage
                .DocumentNode
                .SelectNodes("//ul[@class='description']/li/span");
        if (descriptions != null && descriptions.Count >= DESCRIPTIONS_AMOUNT)
        {
            fields.Add(
                ItemFields.VendorCode,
                descriptions[0].InnerText
            );
            fields.Add(
                ItemFields.Country,
                descriptions[1].InnerText
            );
            fields.Add(
                ItemFields.Producer,
                descriptions[2].InnerText
            );
            if (!fields.ContainsKey(ItemFields.Weight))
            {
                var weightAndUnit = descriptions[3].InnerText.Split();
                fields.Add(
                    ItemFields.Weight,
                    weightAndUnit[0]
                );
                fields.Add(
                    ItemFields.WeightUnit,
                    weightAndUnit[^1]
                );
            }
        }
        else
        {
            _logger.LogWarning("couldn't parse item descriptions");
        }

        var alternativeProducerValue =
            itemPage
                .DocumentNode
                .SelectSingleNode("//div[@class='property_group property_group_10']/table/tbody/tr/td[@class='value']");
        if (alternativeProducerValue != null)
        {
            fields[ItemFields.Producer] = alternativeProducerValue.InnerText.Split(",")[0];
        }
        else
        {
            _logger.LogWarning("couldn't parse alternative producer name");
        }

        var nutrisions =
            itemPage
                .DocumentNode
                .SelectNodes("//div[@class='right']/div/table/tbody/tr/td[@class='value']");
        if (nutrisions != null && nutrisions.Count >= 3)
        {
            fields.Add(
                ItemFields.Protein,
                nutrisions[0].InnerText
            );
            fields.Add(
                ItemFields.Fat,
                nutrisions[1].InnerText
            );
            fields.Add(
                ItemFields.Carbo,
                nutrisions[2].InnerText
            );
            fields.Add(
                ItemFields.Portion,
                STANDART_PORTION_IN_GRAMMS
            );
        }
        else
        {
            _logger.LogWarning("couldn't parse details about portion");
        }

        var compound = SelectFieldWithNodePath(
            itemPage,
            "//div[@class='property_group property_group_8']/table/tbody/tr/td[@class='value']");
        if (compound != null)
        {
            fields.Add(
                ItemFields.Compound,
                compound
            );
        }
        else
        {
            _logger.LogWarning("couldn't parse item compound");
        }

        var categoriesNodes =
            itemPage
                .DocumentNode
                .SelectNodes("//div[@class='breadcrumbs']/ol/li/a/span");
        if (categoriesNodes != null)
        {
            var categories = new StringBuilder(categoriesNodes[^1].InnerText + IItemMapper.CATEGORIES_SEPARATOR);
            categories.Append(categoriesNodes[^2].InnerText + IItemMapper.CATEGORIES_SEPARATOR);
            categories.Append(categoriesNodes[^3].InnerText + IItemMapper.CATEGORIES_SEPARATOR);
            fields.Add(
                ItemFields.Categories,
                categories.ToString()
            );
        }
        else
        {
            _logger.LogWarning("couldn't parse categories info");
        }

        return fields;
    }

    public List<Dictionary<string, string>> ParseItems(string rawItems)
    {
        throw new NotImplementedException();
    }

    private string? SelectFieldWithNodePath(HtmlDocument page, string nodePath)
    {
        return page
                .DocumentNode
                .SelectSingleNode(nodePath)
                ?.InnerText;
    }
}