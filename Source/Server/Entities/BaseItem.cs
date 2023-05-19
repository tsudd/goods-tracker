namespace GoodsTracker.Platform.Server.Entities;

internal sealed class BaseItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? DiscountPrice { get; init; }
    public bool OnDiscount { get; init; }
    public float? Weight { get; init; }
    public string? WeightUnit { get; init; }
    public string? Country { get; init; }
    public string? Currensy { get; init; }
    public string? ImgLink { get; init; }
    public DateTime? FetchDate { get; init; }
    public int VendorId { get; init; }
    public bool IsLiked { get; init; }
}
