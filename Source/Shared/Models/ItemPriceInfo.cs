namespace GoodsTracker.Platform.Shared.Models;

public sealed class ItemPriceInfo
{
    public DateTime FetchDate { get; init; }
    public decimal Price { get; init; }
    public decimal DiscountPrice { get; init; }
}