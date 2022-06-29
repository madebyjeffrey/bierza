using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bierza.Data;

public class Role
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Label { get; set; } = null!;
    public ICollection<User> Users { get; set; } = null!;
}


public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.Label)
            .IsRequired();
    }
}