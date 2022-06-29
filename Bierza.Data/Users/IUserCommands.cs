using Bierza.Data.Models;

namespace Bierza.Data.Users;

public interface IUserCommands
{
    Task<Guid> CreateUser(UserCreateModel model,
        CancellationToken token);

    Task DeleteUser(Guid id, CancellationToken token);
    Task UpdateUser(User user, CancellationToken token);
}