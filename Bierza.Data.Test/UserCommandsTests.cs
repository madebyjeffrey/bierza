using Bierza.Business.UserManagement;
using Bierza.Data.Models;
using Bierza.Data.Users;

namespace Bierza.Data.Test;

public class UserCommandsTests : IDisposable 
{
    private readonly SharedFixture _sharedFixture;
    private readonly UserCommands _userCommands;

    private static CancellationTokenSource _cancellationTokenSource = null!;
    
    public UserCommandsTests()
    {
        _sharedFixture = new();

        _userCommands = new UserCommands(_sharedFixture.DbContext);

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
    
    public async Task CanCreateUser()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test",  "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        
        // Act
        Guid userid = await _userCommands.CreateUser(model, token); 
        
        // Assert
        userid.ShouldNotBe(Guid.Empty);
    }
    
    
    public async Task CannotCreateUserThatAlreadyExists()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test",  "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        
        Guid userid1 = await _userCommands.CreateUser(model, token);

        // Act
        Task<Guid> task = _userCommands.CreateUser(model, token);
        
        // Assert
        await Should.ThrowAsync<UserDataException>(() => task);
    }

    
    public async Task CanDeleteUserThatExists()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test",  "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        
        Guid userid = await _userCommands.CreateUser(model, token);

        // Act
        var task = _userCommands.DeleteUser(userid, token);
        
        // Assert
        await Should.NotThrowAsync(() => task);
    }
    
    
    public async Task CannotDeleteUserThatDoesntExists()
    {
        // Arrange
        CancellationToken token = _cancellationTokenSource.Token;
        
        // Act
        var deleteTask = _userCommands.DeleteUser(Guid.NewGuid(), token);

        // Assert
        await Should.ThrowAsync<UserDataException>(async () => await deleteTask);
    }
}