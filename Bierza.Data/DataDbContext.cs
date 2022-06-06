using Microsoft.EntityFrameworkCore;

namespace Bierza.Data;

public class DataDbContext : DbContext
{
    public DataDbContext() 
    {
    }

    public DataDbContext(DbContextOptions<DataDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hop> Hops => Set<Hop>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataDbContext).Assembly);
    }
}