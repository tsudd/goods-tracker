namespace GoodsTracker.Shared.Model;

public class Item
{
    public const string DEFAULT_ITEM_NAME = "UNNAMED ITEM";
    public string Name { get; set; } = DEFAULT_ITEM_NAME;
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Discount { get; set; }
    public string Link { get; set; } = String.Empty;
    public DateTime? FetchDate { get; set; }
    public string VendorName { get; set; } = String.Empty;

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (object.ReferenceEquals(this, obj))
            return true;
        var item = obj as Item;
        return this.Name == item?.Name
            && this.Price == item?.Price
            && this.Discount == item?.Discount
            && this.Link == item?.Link
            && this.FetchDate == item?.FetchDate
            && this.VendorName == item?.VendorName;
    }

    public override int GetHashCode()
    {
        return ($"{this.Name}{this.Price}{this.Discount}" +
        $"{this.Link}{this.FetchDate}{this.VendorName}").GetHashCode();
    }
}