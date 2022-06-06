using Bierza.Data.Models;
using Bierza.Data.PasswordUtils;
using Bierza.Data.Users;
using Microsoft.Extensions.Options;

namespace Bierza.Data.Test;

public class UserQueriesTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly UserCommands _userCommands;
    private readonly UserQueries _userQueries;

    private static CancellationTokenSource _cancellationTokenSource = null!;
    
    public UserQueriesTests(DatabaseFixture fixture)
    {
        _databaseFixture = fixture;

        IPasswordHasher passwordHasher = new PasswordHasher(new OptionsWrapper<HashingOptions>(new HashingOptions()));

        _userCommands = new UserCommands(_databaseFixture.DbContext, passwordHasher);
        _userQueries = new UserQueries(_databaseFixture.DbContext, passwordHasher);

        _cancellationTokenSource = new CancellationTokenSource();
    }

    [Fact]
    public async Task CanValidateUserWithCorrectPassword()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test", "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        Status<UserCommandResult, Guid> createResult = await _userCommands.CreateUser(model, token);

        Assert.True(createResult.Code == UserCommandResult.Ok);
        ValidateUserModel user = new(model.UserName, model.Password);
        
        // Act
        var result = await _userQueries.ValidateUser(user, token);
        
        // Assert
        Assert.True(result.Code == UserQueryResult.Ok);
        
        // Cleanup
        await _userCommands.DeleteUser(createResult.Data, token);
    }
    
    [Fact]
    public async Task CannotValidateUserWithIncorrectPassword()
    {
        // Arrange
        UserCreateModel model = new("User1", "Test", "Password");
        CancellationToken token = _cancellationTokenSource.Token;
        Status<UserCommandResult, Guid> createResult = await _userCommands.CreateUser(model, token);

        Assert.True(createResult.Code == UserCommandResult.Ok);
        ValidateUserModel user = new(model.UserName, "Blaster");

        Assert.False(model.Password.Equals(user.Password, StringComparison.InvariantCulture));
        
        // Act
        var result = await _userQueries.ValidateUser(user, token);
        
        // Assert
        // Query was okay
        Assert.True(result.Code == UserQueryResult.Ok);

        // Password is not okay
        Assert.False(result.Data!.Valid);
        
        // Cleanup
        await _userCommands.DeleteUser(createResult.Data, token);
    }
    
    [Fact]
    public async Task CanFindUsersByRole()
    {
        // Arrange
        UserCreateModel[] users = 
        {
            new("User1", "Test", "Password"),
            new("User2", "Test", "Password"),
            new("User3", "Test", "Password"),
            new("User4", "NotTest", "Password"),
        };
        
        CancellationToken token = _cancellationTokenSource.Token;

        Stack<Status<UserCommandResult, Guid>> createResults = new();
        
        foreach (var user in users)
        {
            createResults.Push(await _userCommands.CreateUser(user, token));
        }

        Assert.All(createResults, result => Assert.True(result.Code == UserCommandResult.Ok));

        UserQueryByRolesModel model = new("Test");

        // Act
        var result = await _userQueries.GetUsersByRoles(model, token);

        // Assert
        Assert.True(result.Code == UserQueryResult.Ok);
        
        var originalTestUsers = users.Where(x => x.Role == "Test").Select(x => x.UserName).ToHashSet();
        var resultTestUsers = result.Data!.Select(x => x.UserName).ToHashSet();

        resultTestUsers.Should().NotBeEmpty()
            .And.BeEquivalentTo(originalTestUsers);            
        
        // Cleanup
        foreach (var id in createResults.Select(x => x.Data))
        {
            await _userCommands.DeleteUser(id, token);
        }
    }



    
}