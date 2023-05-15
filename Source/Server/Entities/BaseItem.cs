namespace GoodsTracker.Platform.Server.Entities;

public sealed class BaseItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? Discount { get; set; }
    public bool OnDiscount { get; set; }
    public float? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public string? Country { get; set; }
    public string? Currensy { get; set; }
    public string? ImgLink { get; set; }
    public DateTime? FetchDate { get; set; }
    public int VendorId { get; set; }
    public bool IsLiked { get; set; }
}
