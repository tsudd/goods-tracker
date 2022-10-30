namespace GoodsTracker.DataCollector.Models;
public class Item
{
    public const string DEFAULT_ITEM_NAME = "UNNAMED ITEM";
    public string Name1 { get; set; } = DEFAULT_ITEM_NAME;
    public string Name2 { get; set; } = String.Empty;
    public string Name3 { get; set; } = String.Empty;
    public string Price { get; set; } = String.Empty;
    public string Discount { get; set; } = String.Empty;
    public string Country { get; set; } = String.Empty;
    public string Producer { get; set; } = String.Empty;
    public int VendorCode { get; set; } = 0;
    public float Wieght { get; set; } = 0;
    public string WieghtUnit { get; set; } = String.Empty;
    public string Compound { get; set; } = String.Empty;
    public float Protein { get; set; } = 0;
    public float Fat { get; set; } = 0;
    public float Carbo { get; set; } = 0;
    public float Portion { get; set; } = 0;
    public List<string> Categories { get; set; } = new List<string>();
    public string Link { get; set; } = String.Empty;

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (object.ReferenceEquals(this, obj))
            return true;
        var item = obj as Item;
        return this.Name1 == item?.Name1
            && this.Name2 == item?.Name2
            && this.Name3 == item?.Name3
            && this.Price == item?.Price
            && this.Discount == item?.Discount
            && this.Link == item?.Link;
    }

    public override int GetHashCode()
    {
        return $"{this.Name1}{this.Price}{this.Discount}{this.Link}{this.Name2}".GetHashCode();
    }
}
