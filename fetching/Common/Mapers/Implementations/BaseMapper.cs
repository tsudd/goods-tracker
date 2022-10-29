using Common.Mapers.Interfaces;
using Models;
using Models.Constants;

namespace Common.Mapers.Implementations;
public class BasicMapper : IItemMapper
{
    public Item MapItemFields(Dictionary<ItemFields, string> fields)
    {
        return new Item()
        {
            Name1 = fields.GetValueOrDefault(ItemFields.Name1, Item.DEFAULT_ITEM_NAME),
            Name2 = fields.GetValueOrDefault(ItemFields.Name2, string.Empty),
            Name3 = fields.GetValueOrDefault(ItemFields.Name3, string.Empty),
            Price = fields.GetValueOrDefault(ItemFields.Price, string.Empty),
            Discount = fields.GetValueOrDefault(ItemFields.Discount, string.Empty),
            Country = fields.GetValueOrDefault(ItemFields.Country, string.Empty),
            Producer = fields.GetValueOrDefault(ItemFields.Producer, string.Empty),
            VendorCode = ParseIntOrDefault(fields.GetValueOrDefault(ItemFields.VendorCode, "0")),
            Wieght = ParseIntOrDefault(fields.GetValueOrDefault(ItemFields.Weight, "0")),
            WieghtUnit = fields.GetValueOrDefault(ItemFields.WeightUnit, string.Empty),
            Compound = fields.GetValueOrDefault(ItemFields.Compound, string.Empty),
            Carbo = ParseFloatOrDefault(fields.GetValueOrDefault(ItemFields.Name1, "0")),
            Fat = ParseFloatOrDefault(fields.GetValueOrDefault(ItemFields.Name1, "0")),
            Protein = ParseFloatOrDefault(fields.GetValueOrDefault(ItemFields.Protein, "0")),
            Portion = ParseFloatOrDefault(fields.GetValueOrDefault(ItemFields.Portion, "0")),
            Categories = ParseCategoriesOrEmpty(fields.GetValueOrDefault(ItemFields.Categories, string.Empty)),
        };
    }

    protected int ParseIntOrDefault(string numberValue)
    {
        if (Int32.TryParse(numberValue, out int result))
        {
            return result;
        }
        return 0;
    }

    protected float ParseFloatOrDefault(string numberValue)
    {
        if (float.TryParse(
            numberValue,
            System.Globalization.NumberStyles.Float,
            System.Globalization.NumberFormatInfo.InvariantInfo,
            out float result))
        {
            return result;
        }
        return 0;
    }

    protected List<string> ParseCategoriesOrEmpty(string categoriesValue)
    {
        return categoriesValue.Split(IItemMapper.CATEGORIES_SEPARATOR).ToList();
    }

    protected string AdjustPriceIfRequired(string rawPrice)
    {
        return rawPrice.Replace(',', '.');
    }
}