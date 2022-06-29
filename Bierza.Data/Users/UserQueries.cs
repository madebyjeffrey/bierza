using Bierza.Business.UserManagement;
using Bierza.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bierza.Data.Users;

public enum UserQueryResult
{
    Ok,
    TooManyUsersWithUserName,
    InvalidPassword,
    OperationCancelled
}

public class UserQueries : IUserQueries
{
    private readonly DataDbContext _context;

    public UserQueries(DataDbContext context)
    {
        this._context = context;
    }

    public async Task<User> GetUserByGuid(Guid guid, CancellationToken token)
    {
        try
        {
            var result = await this._context.Users
                .Where(x => x.Id == guid)
                .AsNoTracking()
                .SingleAsync(token);

            result.Roles ??= Array.Empty<Role>();

            return result;
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            throw new UserDataException("No User Found", e);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            throw new UserDataException("Operation Cancelled", e);
        }
    }

    public async Task<User> GetUserByEmail(string email, CancellationToken token)
    {
        try
        {
            var result = await this._context.Users
                .Where(x => x.Email.Equals(email.ToLower()))
                .AsNoTracking()
                .SingleAsync(token);

            result.Roles ??= Array.Empty<Role>();
            
            return result;
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            throw new UserDataException("No User Found", e);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            throw new UserDataException("Operation Cancelled", e);
        }
    }

    public async Task<User[]> GetAllUsers(CancellationToken token)
    {
        try
        {
            var result = await this._context.Users.AsNoTrackingWithIdentityResolution()
                .ToArrayAsync(token);
            
            foreach (User user in result)
            {
                user.Roles ??= Array.Empty<Role>();
            }

            return result;
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            throw new UserDataException("Operation Cancelled", e);
        }
    }
}