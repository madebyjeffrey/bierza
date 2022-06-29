using Bierza.Business.UserManagement;
using Bierza.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bierza.Data.Users;

public class UserCommands : IUserCommands
{
    private readonly DataDbContext _context;

    public UserCommands(DataDbContext context)
    {
        this._context = context;
    }

    public async Task<Guid> CreateUser(UserCreateModel model,
        CancellationToken token)
    {
        try
        {
            var user = this._context.Users.Add(new User()
            {
                Email = model.Email.ToLower(),
                DisplayName = model.DisplayName,
                Password = model.Password,
            });

            await this._context.SaveChangesAsync(token);

            return user.Entity.Id;
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            throw new UserDataException("Database Error, possibly duplicate user?", e);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            throw new UserDataException("Operation Cancelled", e);
        }
    }

    public async Task DeleteUser(Guid id, CancellationToken token)
    {
        try
        {
            var result = this._context.Users.Where(x => x.Id == id);

            var user = await result.SingleAsync(token);

            this._context.Users.Remove(user);

            await this._context.SaveChangesAsync(token);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            throw new UserDataException("User not found", e);
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            throw new UserDataException("Database Error", e);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            throw new UserDataException("Operation Cancelled", e);
        }
    }
    
    public async Task UpdateUser(User user, CancellationToken token)
    {
        try
        {
            var result = this._context.Users.Where(x => x.Id == user.Id);
            
            var oldUser = await result.SingleAsync(token);

            oldUser.Activated = user.Activated;
            oldUser.Email = user.Email;
            oldUser.Password = user.Password;
            oldUser.Roles = user.Roles.ToArray();
            oldUser.DisplayName = user.DisplayName;
            oldUser.ValidatedEmail = user.ValidatedEmail;

            var count = await this._context.SaveChangesAsync(token);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            throw new UserDataException("User not found", e);
        }
        catch (Exception e) when (e is DbUpdateException or DbUpdateConcurrencyException)
        {
            throw new UserDataException("Database error", e);
        }
        catch (Exception e) when (e is OperationCanceledException)
        {
            throw new UserDataException("Operation cancelled", e);
        }
    }
}