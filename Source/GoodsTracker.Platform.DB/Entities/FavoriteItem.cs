using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GoodsTracker.Platform.DB.Entities;

[PrimaryKey("ItemId", "UserId")]
public class FavoriteItem
{
    [Required]
    public int ItemId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    // FK
    [Required]
    public Item Item { get; set; } = null!;

    public DateTime DateAdded { get; set; }
}
