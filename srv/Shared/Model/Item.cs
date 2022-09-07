namespace Shared.Model;

public class Item
{
    public const string DEFAULT_ITEM_NAME = "UNNAMED ITEM";
    public string Name1 { get; set; } = DEFAULT_ITEM_NAME;
    public string Name2 { get; set; } = String.Empty;
    public string Name3 { get; set; } = String.Empty;
    public string Price { get; set; } = String.Empty;
    public string Discount { get; set; } = String.Empty;
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