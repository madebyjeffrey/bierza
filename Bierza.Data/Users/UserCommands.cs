using Bierza.Data.Models;
using Bierza.Data.PasswordUtils;
using Microsoft.EntityFrameworkCore;

namespace Bierza.Data.Users;

public enum UserCommandResult
{
    Ok,
    IncorrectEntityCount,
    DatabaseError,
    OperationCancelled,
    UserNotFound,
    ExpectedNoUsers,
    ConflictOnRename
}

public class UserCommands
{
    private readonly DataDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserCommands(DataDbContext context, IPasswordHasher passwordHasher)
    {
        this._context = context;
        this._passwordHasher = passwordHasher;
    }

    public async Task<Status<UserCommandResult, Guid>> CreateUser(UserCreateModel model,
        CancellationToken token)
    {
        try
        {
            var user = this._context.Users.Add(new User()
            {
                UserName = model.UserName,
                Password = _passwordHasher.Hash(model.Password),
                Role = model.Role
            });

            var result = await this._context.SaveChangesAsync(token);

            return result == 1 ? (UserCommandResult.Ok, user.Entity.Id) : UserCommandResult.IncorrectEntityCount;
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            return UserCommandResult.DatabaseError;
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserCommandResult.OperationCancelled;
        }
    }

    public async Task<Status<UserCommandResult>> DeleteUser(Guid id, CancellationToken token)
    {
        try
        {
            var result = this._context.Users.Where(x => x.Id == id);

            var user = await result.SingleAsync(token);

            this._context.Users.Remove(user);

            var count = await this._context.SaveChangesAsync(token);

            if (count != 1)
            {
                return UserCommandResult.IncorrectEntityCount;
            }

            return UserCommandResult.Ok; 
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return UserCommandResult.UserNotFound;
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            return UserCommandResult.DatabaseError;
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserCommandResult.OperationCancelled;
        }
    }
    
    public async Task<UserCommandResult> UpdateUserPassword(UserPasswordUpdateModel model, CancellationToken token)
    {
        try
        {
            var result = this._context.Users.Where(x => x.Id == model.Id);
            
            var user = await result.SingleAsync(token);

            user.Password = _passwordHasher.Hash(model.Password);

            var count = await this._context.SaveChangesAsync(token);

            if (count != 1)
            {
                return UserCommandResult.IncorrectEntityCount;
            }

            return UserCommandResult.Ok; 
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return UserCommandResult.UserNotFound;
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            return UserCommandResult.DatabaseError;
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserCommandResult.OperationCancelled;
        }
    }
    
    public async Task<UserCommandResult> UpdateUser(UserUpdateModel model, CancellationToken token)
    {
        try
        {
            var result = this._context.Users.Where(x => x.Id == model.Id);

            var user = await result.SingleAsync(token);

            // Check to see if renaming the user would conflict with an existing user
            if (!user.UserName.Equals(model.UserName, StringComparison.InvariantCultureIgnoreCase))
            {
                var users = this._context.Users.Where(x => x.UserName == model.UserName).AsNoTracking();

                if (await users.CountAsync(token) != 0)
                {
                    return UserCommandResult.ConflictOnRename;
                }
            }

            // username can change if required
            user.UserName = model.UserName;
            user.Role = model.Role;

            var count = await this._context.SaveChangesAsync(token);

            if (count != 1)
            {
                return UserCommandResult.IncorrectEntityCount;
            }

            return UserCommandResult.Ok;
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return UserCommandResult.UserNotFound;
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            return UserCommandResult.DatabaseError;
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            return UserCommandResult.OperationCancelled;
        }
    }
}