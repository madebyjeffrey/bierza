using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bierza.Data;

public class Hop
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Name { get; set; } = String.Empty;
    public string Origin { get; set; } = String.Empty;

    public bool UsedForBittering { get; set; } = default;
    public bool UsedForAroma { get; set; } = default;

    public double AlphaAcid { get; set; } = default;
    public double? BetaAcid { get; set; } = default;

    public string Notes { get; set; } = String.Empty;
}

public class HopEntityConfiguration : IEntityTypeConfiguration<Hop>
{
    public void Configure(EntityTypeBuilder<Hop> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.Name)
            .IsUnicode()
            .IsRequired();

        builder.Property(b => b.Origin)
            .IsUnicode()
            .IsRequired(false);

        builder.Property(b => b.UsedForBittering)
            .IsRequired();

        builder.Property(b => b.UsedForAroma)
            .IsRequired();

        builder.Property(b => b.AlphaAcid)
            .IsRequired()
            .HasColumnType("decimal(5,4)");

        builder.Property(b => b.BetaAcid)
            .IsRequired(false)
            .HasColumnType("decimal(5,4)");

        builder.Property(b => b.Notes)
            .IsRequired(false);
    }
}