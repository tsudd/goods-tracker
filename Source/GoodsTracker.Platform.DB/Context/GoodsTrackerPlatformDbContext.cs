using GoodsTracker.Platform.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Stream = System.IO.Stream;

namespace GoodsTracker.Platform.DB.Context;

public class GoodsTrackerPlatformDbContext : DbContext
{
    public GoodsTrackerPlatformDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Vendor> Vendors { get; set; } = null!;
    public DbSet<Stream> Streams { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<ItemRecord> ItemRecords { get; set; } = null!;
    public DbSet<FavoriteItem> FavoriteItems { get; set; } = null!;
    public DbSet<Producer> Producers { get; set; } = null!;
    public DbSet<ItemError> ItemErrors { get; set; } = null!;
}