using GoodsTracker.DataCollector.Common.Mappers.Interfaces;
using GoodsTracker.DataCollector.Models;
using GoodsTracker.DataCollector.Models.Constants;

namespace GoodsTracker.DataCollector.Common.Mappers.Implementations;
public class BasicMapper : IItemMapper
{
    public Item MapItemFields(Dictionary<ItemFields, string> fields)
    {
        Func<string, string?> noAffect = static _ => _;
        return new Item()
        {
            Name1 = fields.GetValueOrDefault(ItemFields.Name1, Item.DEFAULT_ITEM_NAME),
            Name2 = TryGetValueOrDefault(fields, ItemFields.Name2, noAffect),
            Name3 = TryGetValueOrDefault(fields, ItemFields.Name3, noAffect),
            Price = TryGetValueOrDefault(fields, ItemFields.Price, AdjustPriceIfRequired),
            Discount = TryGetValueOrDefault(fields, ItemFields.Discount, AdjustPriceIfRequired),
            Country = TryGetValueOrDefault(fields, ItemFields.Country, noAffect),
            Producer = TryGetValueOrDefault(fields, ItemFields.Producer, noAffect),
            VendorCode = TryGetValueOrDefault(fields, ItemFields.VendorCode, ParseIntOrDefault),
            Wieght = TryGetValueOrDefault(fields, ItemFields.Weight, ParseFloatOrDefault),
            WieghtUnit = TryGetValueOrDefault(fields, ItemFields.WeightUnit, noAffect),
            Compound = TryGetValueOrDefault(fields, ItemFields.Compound, noAffect),
            Carbo = TryGetValueOrDefault(fields, ItemFields.Carbo, ParseFloatOrDefault),
            Fat = TryGetValueOrDefault(fields, ItemFields.Fat, ParseFloatOrDefault),
            Protein = TryGetValueOrDefault(fields, ItemFields.Protein, ParseFloatOrDefault),
            Portion = TryGetValueOrDefault(fields, ItemFields.Portion, ParseFloatOrDefault),
            Categories = TryGetValueOrDefault(fields, ItemFields.Categories, ParseCategoriesOrEmpty),
        };
    }

    protected int? ParseIntOrDefault(string numberValue)
    {
        if (Int32.TryParse(numberValue, out int result))
        {
            return result;
        }
        return null;
    }

    protected float? ParseFloatOrDefault(string numberValue)
    {
        if (float.TryParse(
            numberValue,
            System.Globalization.NumberStyles.Float,
            System.Globalization.NumberFormatInfo.InvariantInfo,
            out float result))
        {
            return result;
        }
        return null;
    }

    protected List<string> ParseCategoriesOrEmpty(string categoriesValue)
    {
        return categoriesValue.Split(IItemMapper.CATEGORIES_SEPARATOR).ToList();
    }

    protected string AdjustPriceIfRequired(string rawPrice)
    {
        return rawPrice.Replace(',', '.');
    }

    protected TValue? TryGetValueOrDefault<TValue>(
        Dictionary<ItemFields, string> dict,
        ItemFields field,
        Func<string, TValue?> affect)
    => dict.ContainsKey(field) ? affect(dict[field]) : default(TValue);
}