namespace Models;
public class Item
{
    public const string DEFAULT_ITEM_NAME = "UNNAMED ITEM";
    public string Name { get; set; } = DEFAULT_ITEM_NAME;
    public double Price { get; set; } = 0;
    public double Discount { get; set; } = 0;
    public string Link { get; set; } = "";
}
