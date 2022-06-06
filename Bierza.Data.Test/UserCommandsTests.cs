using Bierza.Data.Models;
using Bierza.Data.PasswordUtils;
using Bierza.Data.Users;
using Microsoft.Extensions.Options;

namespace Bierza.Data.Test;

public class UserCommandsTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly UserCommands _userCommands;

    private static CancellationTokenSource _cancellationTokenSource = null!;
    
    public UserCommandsTests(DatabaseFixture fixture)
    {
        _databaseFixture = fixture;

        IPasswordHasher passwordHasher = new PasswordHasher(new OptionsWrapper<HashingOptions>(new HashingOptions()));

        _userCommands = new UserCommands(_databaseFixture.DbContext, passwordHasher);

        _cancellationTokenSource = new CancellationTokenSource();
    }
    
    [Fact]
    public async Task CanCreateUser()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test", "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        
        // Act
        Status<UserCommandResult, Guid> result = await _userCommands.CreateUser(model, token);
        
        // Assert
        Assert.True(result.Code == UserCommandResult.Ok);
        
        // Cleanup
        await _userCommands.DeleteUser(result.Data, token);
    }
    
    [Fact]
    public async Task CannotCreateUserThatAlreadyExists()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test", "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        
        Status<UserCommandResult, Guid> result1 = await _userCommands.CreateUser(model, token);

        Assert.True(result1.Code == UserCommandResult.Ok);
        
        // Act
        Status<UserCommandResult, Guid> result2 = await _userCommands.CreateUser(model, token);
        
        // Assert
        Assert.True(result2.Code == UserCommandResult.DatabaseError);
        
        // Cleanup
        await _userCommands.DeleteUser(result1.Data, token);
    }

    [Fact]
    public async Task CanDeleteUserThatExists()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test", "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        
        Status<UserCommandResult, Guid> result = await _userCommands.CreateUser(model, token);

        Assert.True(result.Code == UserCommandResult.Ok);
        
        // Act
        var deleteResult = await _userCommands.DeleteUser(result.Data, token);

        // Assert
        Assert.True(deleteResult.Code == UserCommandResult.Ok);
    }
    
    [Fact]
    public async Task CannotDeleteUserThatDoesntExists()
    {
        // Arrange
        CancellationToken token = _cancellationTokenSource.Token;
        
        // Act
        var deleteResult = await _userCommands.DeleteUser(Guid.NewGuid(), token);

        // Assert
        Assert.True(deleteResult.Code == UserCommandResult.UserNotFound);
    }
}