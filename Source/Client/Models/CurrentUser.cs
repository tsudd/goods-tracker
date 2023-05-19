namespace GoodsTracker.Platform.Client.Models;

internal class CurrentUser
{
    internal static bool IsAuthenticated { get; set; }
    internal static string? UserName { get; set; }
    public Dictionary<string, string>? Claims { get; set; }
    public static string? UserId { get; set; }
    internal const string Uid = "EpVNlJlQ3Ra9m8Lw0Ie348BflZg1";
    internal const string Role = "Admin";
}
