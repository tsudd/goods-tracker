using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodsTracker.Platform.DB.Entities;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public uint Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // FK
    public ICollection<Item> Items { get; set; } = new List<Item>();

    public sealed class EntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasIndex(static x => x.Name)
                   .HasMethod("gin")
                   .HasOperators("gin_trgm_ops");
        }
    }
}
