using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsTracker.Platform.DB.Entities;

public class Stream
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime FetchDate { get; set; }
}
