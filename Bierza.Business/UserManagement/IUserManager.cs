using System.Collections.Immutable;
using Bierza.Business.UserManagement.Models;

namespace Bierza.Business.UserManagement;

public interface IUserManager
{
    Task<Guid> CreateUser(CreateUserRequestModel user, CancellationToken cancellationToken);
    Task<AuthenticationResponse?> AuthenticateUser(AuthenticationRequest model, CancellationToken token);
    Task<UserReadModel?> ValidateToken(string token, CancellationToken cancellationToken);
    Task<ImmutableArray<UserReadModel>> GetAllUsers(CancellationToken token);
}