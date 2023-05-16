namespace GoodsTracker.Platform.Server.Entities;

public sealed class BaseInfo
{
    internal int ItemsCount { get; init; }
    internal List<string> ShopsColumns { get; init; } = new();
}
