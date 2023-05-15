using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoodsTracker.Platform.DB.Entities.Enumerators;

namespace GoodsTracker.Platform.DB.Entities;

public class ItemError
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }

    [Required]
    public ErrorType ErrorType { get; set; }

    [Required]
    public string Details { get; set; } = null!;

    public string? SerialiedItem { get; set; }

    public int StreamId { get; set; }
    public bool Resolved { get; set; }

    [Required]
    public Stream Stream { get; set; } = null!;
}
