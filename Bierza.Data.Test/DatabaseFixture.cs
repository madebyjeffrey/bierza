namespace Bierza.Data.Test;

public class DatabaseFixture : IDisposable
{
    public DataDbContext DbContext { get; init; }

    public DatabaseFixture()
    {
        DbContext = new TestDbContext();
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
    }
}