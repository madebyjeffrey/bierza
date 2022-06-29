using Bierza.Data.Users;

namespace Bierza.Data.Test;

public class UserQueriesTests : IDisposable
{
    private readonly SharedFixture _sharedFixture;
    private readonly UserCommands _userCommands;
    private readonly UserQueries _userQueries;

    private static CancellationTokenSource _cancellationTokenSource = null!;
    
    public UserQueriesTests()
    {
        _sharedFixture = new();

        _userCommands = new UserCommands(_sharedFixture.DbContext);
        _userQueries = new UserQueries(_sharedFixture.DbContext);

        _cancellationTokenSource = new CancellationTokenSource();
    }
    
    public void Dispose()
    {
        Dispose(true);
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
            this._sharedFixture.Dispose();
        }

        this._disposed = true;
    }
}