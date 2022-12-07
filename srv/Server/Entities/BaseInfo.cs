namespace GoodsTracker.Platform.Server.Entities;

public sealed class BaseInfo
{
    public int ItemsCount { get; set; }
    public List<string> ShopsColumns { get; set; } = new();
}