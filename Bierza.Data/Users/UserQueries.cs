using Bierza.Data.Models;
using Bierza.Data.PasswordUtils;
using Microsoft.EntityFrameworkCore;

namespace Bierza.Data.Users;

public enum UserQueryResult
{
    Ok,
    TooManyUsersWithUserName,
    InvalidPassword,
    OperationCancelled
}

public class UserQueries
{
    private readonly DataDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserQueries(DataDbContext context, IPasswordHasher passwordHasher)
    {
        this._context = context;
        this._passwordHasher = passwordHasher;
    }

    public async Task<Status<UserQueryResult, UserValidationModel>> ValidateUser(ValidateUserModel model,
        CancellationToken token)
    {
        try
        {
            var result = this._context.Users.Where(x => x.UserName == model.UserName).AsNoTracking();

            if (await result.CountAsync(token) != 1)
            {
                return UserQueryResult.TooManyUsersWithUserName;
            }

            var user = await result.AsNoTracking().FirstAsync(token);
            
            var (valid, upgrade) = this._passwordHasher.Check(user.Password, model.Password);

            return (UserQueryResult.Ok, new UserValidationModel(valid, upgrade));
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserQueryResult.OperationCancelled;
        }
    }

    public async Task<Status<UserQueryResult, UserReadModel[]>> GetUsersByRoles(UserQueryByRolesModel model, CancellationToken token)
    {
        try
        {
            var result = await this._context.Users.AsNoTracking()
                .Where(x => model.Roles.Contains(x.Role))
                .Select(x => new UserReadModel(x.Id, x.UserName, x.Role))
                .ToArrayAsync(token);

            return (UserQueryResult.Ok, result);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserQueryResult.OperationCancelled;
        }
    }

    public async Task<Status<UserQueryResult, UserReadModel[]>> GetUsers(CancellationToken token)
    {
        try
        {
            var result = await this._context.Users.AsNoTracking()
                .Select(x => new UserReadModel(x.Id, x.UserName, x.Role))
                .ToArrayAsync(token);

            return (UserQueryResult.Ok, result);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserQueryResult.OperationCancelled;
        }
    }
}