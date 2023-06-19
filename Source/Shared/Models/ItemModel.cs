namespace GoodsTracker.Platform.Shared.Models;

public sealed class ItemModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal DiscountPrice { get; init; }
    public int Discount { get; init; }
    public bool OnDiscount { get; init; }
    public float? Weight { get; init; }
    public string? WeightUnit { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public string ImgLink { get; init; } = string.Empty;
    public DateTime FetchDate { get; init; }
    public int VendorId { get; init; }
    public string? VendorName { get; init; } = string.Empty;
    public bool Liked { get; init; }
    public string? Compound { get; init; } = string.Empty;
    public float? Protein { get; init; }
    public float? Fat { get; init; }
    public float? Carbo { get; init; }
    public string ProducerName { get; init; } = string.Empty;
    public long? VendorCode { get; init; }
    public IEnumerable<string> Categories { get; init; } = new List<string>();
    public float? Portion { get; init; }
    public bool Adult { get; init; }
    
    public IEnumerable<ItemPriceInfo> PriceHistory { get; init; } = Array.Empty<ItemPriceInfo>();
}