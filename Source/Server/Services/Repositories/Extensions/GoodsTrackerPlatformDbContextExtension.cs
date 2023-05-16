namespace GoodsTracker.Platform.Server.Services.Repositories.Extensions;

using GoodsTracker.Platform.DB.Context;
using GoodsTracker.Platform.DB.Entities;

internal static class GoodsTrackerPlatformDbContextExtension
{
    internal static IQueryable<ItemRecord> GetItemRecords(this GoodsTrackerPlatformDbContext context, bool discountsOnly)
    {
        return discountsOnly ? context.ItemRecords.Where(static i => i.OnDiscount) : context.ItemRecords;
    }
}
