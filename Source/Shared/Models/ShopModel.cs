using System.Globalization;

namespace GoodsTracker.Platform.Shared.Models;

public sealed class ShopModel
{
    public int Id { get; init; }
    public string Name1 { get; init; } = String.Empty;
    public string Name2 { get; init; } = String.Empty;

    public string GetShopNameWithCulture(CultureInfo culture)
    {
        if (culture.DisplayName == "en-US")
            return Name2;
        return Name1;
    }
}