using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bierza.Data.Test;

public class TestDbContext : DataDbContext
{
    private readonly string connectionString;
    
    public TestDbContext()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .AddEnvironmentVariables() 
            .Build();

        connectionString = config["ConnectionStrings:DefaultConnection"];
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(connectionString);
}