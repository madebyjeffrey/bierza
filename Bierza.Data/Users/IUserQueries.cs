using Bierza.Data.Models;

namespace Bierza.Data.Users;

public interface IUserQueries
{
    Task<User> GetUserByGuid(Guid guid, CancellationToken token);
    Task<User[]> GetAllUsers(CancellationToken token);
    Task<User> GetUserByEmail(string email, CancellationToken token);
}