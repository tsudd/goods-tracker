namespace GoodsTracker.Platform.Shared.Models;

public sealed class InfoModel
{
    public int ItemsCount { get; init; }
    public IEnumerable<ShopModel>? Shops { get; init; }
}