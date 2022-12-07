namespace GoodsTracker.Platform.Shared.Models;

public sealed class BaseItemModel
{
    public const string DEFAULT_ITEM_NAME = "UNNAMED ITEM";
    public int Id { get; init; }
    public string Name { get; init; } = DEFAULT_ITEM_NAME;
    public decimal Price { get; init; }
    public decimal DiscountPrice { get; init; }
    public int Discount { get; init; }
    public bool OnDiscount { get; init; }
    public float Weight { get; init; }
    public string WeightUnit { get; init; } = String.Empty;
    public string Country { get; init; } = String.Empty;
    public string Currensy { get; init; } = String.Empty;
    public string ImgLink { get; init; } = String.Empty;
    public DateTime FetchDate { get; init; }
    public string VendorName { get; init; } = String.Empty;
}