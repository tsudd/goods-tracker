using System.ComponentModel.DataAnnotations;

namespace GoodsTracker.Platform.Client.Models;

public class LoginRequestModel
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}