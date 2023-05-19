namespace GoodsTracker.Platform.Server.Services.Repositories;

using GoodsTracker.Platform.DB.Context;
using GoodsTracker.Platform.Server.Entities;

using Microsoft.EntityFrameworkCore;

internal sealed class FavoritesRepository
{
    private readonly GoodsTrackerPlatformDbContext context;

    public FavoritesRepository(GoodsTrackerPlatformDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<BaseItem>> GetUserFavoritesAsync(string userId)
    {
        var favoriteItemsWithLatestRecords = await (
                from favorite in this.context.FavoriteItems.Include(static fi => fi.Item)
                                     .ThenInclude(static i => i.Vendor)
                where favorite.UserId == userId
                join producer in this.context.Producers on favorite.Item.ProducerId equals producer.Id into
                    producerGroup
                from producer in producerGroup.DefaultIfEmpty()
                join itemRecord in this.context.ItemRecords.Include(static r => r.Stream) on favorite.ItemId equals
                    itemRecord.ItemId
                let latestItemFetchDate = this.context.ItemRecords.Where(i => i.ItemId == favorite.ItemId)
                                              .Max(static i => i.Stream.FetchDate)
                where itemRecord.Stream.FetchDate == latestItemFetchDate
                select new
                {
                    Id = itemRecord.ItemId,
                    itemRecord.Price,
                    itemRecord.CutPrice,
                    itemRecord.OnDiscount,
                    Name = favorite.Item.Name1,
                    ImgLink = favorite.Item.ImageLink,
                    favorite.Item.Weight,
                    favorite.Item.WeightUnit,
                    Currensy = favorite.Item.Vendor.Land,
                    favorite.Item.VendorId,
                    itemRecord.Stream.FetchDate,
                    Liked = true,
                    producer.Country,
                }).ToListAsync()
                  .ConfigureAwait(false);

        return favoriteItemsWithLatestRecords.Select(
            static r => new BaseItem
            {
                Id = r.Id,
                Name = r.Name,
                Currensy = r.Currensy,
                FetchDate = r.FetchDate,
                Price = r.Price,
                VendorId = r.VendorId,
                Weight = r.Weight,
                WeightUnit = r.WeightUnit,
                OnDiscount = r.OnDiscount,
                DiscountPrice = r.CutPrice,
                ImgLink = r.ImgLink?.ToString(),
                Country = r.Country,
                IsLiked = r.Liked,
            });
    }
}
