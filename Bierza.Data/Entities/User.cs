using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bierza.Data;

public class User
{
    public Guid Id { get; set; } = Guid.Empty;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.UserName)
            .IsRequired();

        builder.HasIndex(user => user.UserName, "UserNameUnique")
            .IsUnique(true);

        builder.Property(b => b.Password)
            .IsRequired();

        builder.Property(b => b.Role)
            .IsRequired();
    }
}