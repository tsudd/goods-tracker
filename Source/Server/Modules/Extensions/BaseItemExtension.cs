namespace GoodsTracker.Platform.Server.Modules.Extensions;

using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Modules.Helpers;
using GoodsTracker.Platform.Shared.Constants;
using GoodsTracker.Platform.Shared.Models;

internal static class BaseItemExtension
{
    internal static BaseItemModel ToModel(this BaseItem baseItem)
    {
        return new BaseItemModel
        {
            Id = baseItem.Id,
            Name = baseItem.Name,
            Price = baseItem.Price,
            DiscountPrice = baseItem.DiscountPrice ?? 0,
            Discount = PriceHelper.CalculateDiscount(baseItem.DiscountPrice, baseItem.Price),
            OnDiscount = baseItem.OnDiscount,
            Country = baseItem.Country ?? GoodsTrackerDefaults.DefaultCountry,
            Currensy = baseItem.Currensy ?? GoodsTrackerDefaults.DefaultCurrency,
            FetchDate = baseItem.FetchDate ?? GoodsTrackerDefaults.DefaultFetchDate,
            ImgLink = baseItem.ImgLink ?? GoodsTrackerDefaults.DefaultImgLink,
            VendorId = baseItem.VendorId,
            Weight = baseItem.Weight ?? 0,
            WeightUnit = baseItem.WeightUnit ?? GoodsTrackerDefaults.DefaultWeightUnit,
            Liked = baseItem.IsLiked,
        };
    }
}
