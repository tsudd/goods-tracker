namespace GoodsTracker.Platform.Shared.Constants;

public static class GoodsTrackerDefaults
{
    public const string GoodsTrackerApiV1 = "api/v1";

    public const string DefaultCurrency = "BYN";
    public const string DefaultWeightUnit = "g";
    public const string DefaultCountry = "Belarus";
    public const string DefaultImgLink = "img/no_image.png";
    public static DateTime DefaultFetchDate => DateTime.Today;
    
    public const string ItemModuleRoute = "items";
    public const string FavoritesModuleRoute = "favorites";
}