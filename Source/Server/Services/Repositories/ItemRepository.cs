using GoodsTracker.Platform.DB.Context;
using GoodsTracker.Platform.Server.Entities;
using GoodsTracker.Platform.Server.Services.Repositories.Abstractions;
using GoodsTracker.Platform.Server.Services.Repositories.Enumerators;

namespace GoodsTracker.Platform.Server.Services.Repositories;

using GoodsTracker.Platform.DB.Entities;
using GoodsTracker.Platform.Server.Services.Repositories.Extensions;

using Microsoft.EntityFrameworkCore;

// TODO: move free-text search into other method and query (do fuzzy search first and then joins)
internal sealed class ItemRepository : IItemRepository
{
    private readonly GoodsTrackerPlatformDbContext context;

    public ItemRepository(GoodsTrackerPlatformDbContext context)
    {
        this.context = context;
    }

    // TODO: make this method more safe
    public async Task<BaseInfo> GetItemsInfoAsync()
    {
        var vendorsInfo = await this.context.Vendors.Select(
                                        static v => new
                                        {
                                            v.Id,
                                            v.Name1,
                                            v.Name2,
                                        })
                                    .ToListAsync()
                                    .ConfigureAwait(false);

        return new BaseInfo
        {
            ItemsCount = this.context.Items.Count(),
            ShopsColumns = vendorsInfo.Select(static v => $"{v.Id},{v.Name1},{v.Name2}")
                                      .ToList(),
        };
    }

    public async Task<IEnumerable<BaseItem>> GetItemsByGroupsAsync(
        int page, int amount, ItemsOrder order, int vendorFilterId,
        bool discountOnly, string? userId = null, string? q = null)
    {
        var query =
            from record in this.context.GetItemRecords(discountOnly)
            join stream in this.context.Streams on record.StreamId equals stream.Id
            join item in this.context.Items on record.ItemId equals item.Id
            join producer in this.context.Producers on item.ProducerId equals producer.Id into producerGroup
            from producer in producerGroup.DefaultIfEmpty()
            join favoriteItem in this.context.FavoriteItems on new
            {
                Key1 = record.ItemId,
                Key2 = userId,
            } equals new
            {
                Key1 = favoriteItem.ItemId,
                Key2 = favoriteItem.UserId,
            } into favoriteGroup
            from favoriteItem in favoriteGroup.DefaultIfEmpty()
            join vendor in this.context.Vendors on item.VendorId equals vendor.Id
            where !this.context.ItemRecords.Any(
                ir => ir.ItemId == record.ItemId && ir.Stream.FetchDate > stream.FetchDate)
            select new
            {
                Id = record.ItemId,
                record.Price,
                record.CutPrice,
                record.OnDiscount,
                Name = item.Name1,
                ImgLink = item.ImageLink,
                item.Weight,
                item.WeightUnit,
                Currensy = vendor.Land,
                item.VendorId,
                item.ImageLink,
                stream.FetchDate,
                producer.Country,
                IsLiked = favoriteItem != null,
            };

        // TODO: try to move it into an extension methods. Reflection possible option
        var searchableQuery = q != null ? query.Where(i => EF.Functions.ILike(i.Name, q)) : query;
        var filteredByVendor = vendorFilterId > 0 ? searchableQuery.Where(i => i.VendorId == vendorFilterId) : searchableQuery;

        var orderedQuery = order switch
        {
            ItemsOrder.ByLastUpdateDate => filteredByVendor.OrderByDescending(static i => i.FetchDate),
            ItemsOrder.CheapFirst => filteredByVendor.OrderBy(static i => i.Price),
            ItemsOrder.ExpensiveFirst => filteredByVendor.OrderByDescending(static i => i.Price),
            var _ => filteredByVendor.OrderByDescending(static i => i.Id),
        };

        var result = await orderedQuery
                           .Skip(page)
                           .Take(amount)
                           .ToListAsync().ConfigureAwait(false);

        return result.Select(
            static r => new BaseItem
            {
                Id = r.Id,
                Name = r.Name,
                Currensy = r.Currensy,
                FetchDate = r.FetchDate,
                Discount = CalculateDiscount(r.CutPrice, r.Price),
                Price = r.Price,
                VendorId = r.VendorId,
                Weight = r.Weight,
                WeightUnit = r.WeightUnit,
                OnDiscount = r.OnDiscount,
                DiscountPrice = r.CutPrice,
                ImgLink = r.ImageLink?.ToString(),
                Country = r.Country,
                IsLiked = r.IsLiked,
            });
    }

    private static int CalculateDiscount(decimal? cutPrice, decimal price)
    {
        return (int)Math.Round((1 - (cutPrice ?? 0) / price) * 100);
    }

    // TODO: rewrite to use Result
    public async Task<bool> AddUserFavoriteItemAsync(int itemId, string userId, DateTime dateTime)
    {
        this.context.Add(
            new FavoriteItem
            {
                ItemId = itemId,
                UserId = userId,
                DateAdded = dateTime,
            });

        await this.context.SaveChangesAsync()
                  .ConfigureAwait(false);

        return true;
    }

    // TODO: implement checks instead of exceptions handling
    public async Task<bool> DeleteUserFavoriteItemAsync(int itemId, string userId)
    {
        var favoriteItem = new FavoriteItem
        {
            ItemId = itemId,
            UserId = userId,
        };

        this.context.Attach(favoriteItem);
        this.context.FavoriteItems.Remove(favoriteItem);
        await this.context.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public bool HasAll(params int[] itemIds)
    {
        return this.context.Items.Count(x => itemIds.Contains(x.Id)) ==
               itemIds.Distinct()
                      .Count();
    }
}
