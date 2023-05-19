namespace GoodsTracker.Platform.Shared.Models;

public sealed class ItemModel
{
    public int Id { get; init; }
    public string Name { get; init; }
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
    public int VendorId { get; init; }
    public bool Liked { get; init; }
    public string Compound { get; init; } = String.Empty;
    public float Protein { get; init; }
    public float Fat { get; init; }
    public float Carbo { get; init; }
    public string ProducerName { get; init; } = String.Empty;
    public int VendorCode { get; init; }
    public IEnumerable<string> Categories { get; init; } = new List<string>();
    public float Portion { get; init; }
    public bool Adult { get; init; }
}