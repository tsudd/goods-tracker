namespace GoodsTracker.Platform.Server.Modules.Extensions;

using GoodsTracker.Platform.DB.Entities;
using GoodsTracker.Platform.Server.Modules.Helpers;
using GoodsTracker.Platform.Shared.Constants;
using GoodsTracker.Platform.Shared.Models;

internal static class ItemExtensions
{
    internal static ItemModel ToModel(this Item item, ItemRecord latestItemPrice)
    {
        return new ItemModel
        {
            Name = item.Name1,
            Id = item.Id,
            Adult = item.Adult,
            Carbo = item.Carbo,
            Fat = item.Fat,
            Portion = item.Portion,
            Protein = item.Protein,
            Categories = item.Categories.Select(static c => c.Name),
            Compound = item.Compound,
            Weight = item.Weight,
            WeightUnit = item.WeightUnit,
            Country = item.Producer?.Country ?? GoodsTrackerDefaults.DefaultCountry,
            Currency = item.Vendor.Land ?? GoodsTrackerDefaults.DefaultCurrency,
            Price = latestItemPrice.Price,
            DiscountPrice = latestItemPrice.CutPrice ?? 0,
            Discount = PriceHelper.CalculateDiscount(latestItemPrice.CutPrice, latestItemPrice.Price),
            OnDiscount = latestItemPrice.OnDiscount,
            FetchDate = latestItemPrice.Stream.FetchDate,
            ImgLink = item.ImageLink?.ToString() ?? GoodsTrackerDefaults.DefaultImgLink,
            VendorId = item.Vendor.Id,
            VendorName = item.Vendor.Name1,
            VendorCode = item.VendorCode,
            PriceHistory = item.PriceRecords.Select(
                static record => new ItemPriceInfo
                {
                    Price = record.Price,
                    DiscountPrice = record.CutPrice ?? 0,
                    FetchDate = record.Stream.FetchDate,
                }),
            Liked = false,
        };
    }
}
