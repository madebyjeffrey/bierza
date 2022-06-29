using Bierza.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bierza.Data;

public class DataDbContext : DbContext
{
    private readonly string _connectionString;
    public DataDbContext(ConnectionString connectionString)
    {
        this._connectionString = connectionString.Connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_connectionString, x => x.MigrationsAssembly("Bierza.Data"));
    
    public DbSet<Hop> Hops => Set<Hop>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataDbContext).Assembly);
    }
}