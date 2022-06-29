using Bierza.Data.Models;
using Microsoft.Extensions.Configuration;

namespace Bierza.Data.Test;

public class SharedFixture : IDisposable
{
    public DataDbContext DbContext { get; set; }
    private static string ConnectionString { get; set; }

    static SharedFixture() {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .AddEnvironmentVariables() 
            .Build();

        SharedFixture.ConnectionString = config["ConnectionStrings:DefaultConnection"];
    }

    public SharedFixture()
    {
        DbContext = new DataDbContext(new ConnectionString(ConnectionString));
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }

        DbContext = null!;

        _disposed = true;
    }
}