namespace GoodsTracker.Platform.Server.Modules.Item.Extensions;

using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Shared.Models;
using GoodsTracker.Platform.Shared.Constants;

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
            Discount = baseItem.Discount ?? 0,
            OnDiscount = baseItem.OnDiscount,
            Country = baseItem.Country ?? GoodsTrackerDefaults.DefaultCountry,
            Currensy = baseItem.Currensy ?? GoodsTrackerDefaults.DefaultCurrency,
            FetchDate = baseItem.FetchDate ?? GoodsTrackerDefaults.DefaultFetchDate,
            ImgLink = baseItem.ImgLink ?? GoodsTrackerDefaults.DefaultImgLink,
            VendorId = baseItem.VendorId,
            Weight = baseItem.Weight ?? 0,
            WeightUnit = baseItem.WeightUnit ?? GoodsTrackerDefaults.DefaultImgLink,
            Liked = baseItem.IsLiked,
        };
    }
}
