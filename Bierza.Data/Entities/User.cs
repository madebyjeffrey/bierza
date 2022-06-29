using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bierza.Data;

public class User
{
    public Guid Id { get; set; } = Guid.Empty;
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool ValidatedEmail { get; set; } = false;
    public bool Activated { get; set; } = false;
    public ICollection<Role>? Roles { get; set; } = null!;

}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.DisplayName)
            .IsRequired();

        builder.Property(b => b.Email)
            .IsRequired();

        builder.HasIndex(user => user.DisplayName, "DisplayNameUnique")
            .IsUnique(true);
        
        builder.HasIndex(user => user.Email, "EmailUnique")
            .IsUnique(true);

        builder.Property(b => b.Password)
            .IsRequired();

        builder.Property(b => b.ValidatedEmail)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.Activated)
            .IsRequired()
            .HasDefaultValue(false);
    }
}