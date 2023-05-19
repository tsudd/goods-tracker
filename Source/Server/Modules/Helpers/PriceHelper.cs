namespace GoodsTracker.Platform.Server.Modules.Helpers;

internal static class PriceHelper
{
    public static int CalculateDiscount(decimal? cutPrice, decimal price)
    {
        return (int)Math.Round((1 - (cutPrice ?? 0) / price) * 100);
    }
}
