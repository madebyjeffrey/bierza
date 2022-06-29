using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bierza.Business.Models;
using Bierza.Business.PasswordUtils;
using Bierza.Business.UserManagement.Models;
using Bierza.Data;
using Bierza.Data.Models;
using Bierza.Data.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bierza.Business.UserManagement;

public class UserManager : IUserManager
{
    private readonly IUserCommands _userCommands;
    private readonly IUserQueries _userQueries;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;
    
    public UserManager(IUserCommands userCommands, IUserQueries userQueries, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings)
    {
        this._userCommands = userCommands;
        this._userQueries = userQueries;
        this._passwordHasher = passwordHasher;
        this._jwtSettings = jwtSettings.Value;
    }

    public async Task<Guid> CreateUser(CreateUserRequestModel user, CancellationToken cancellationToken)
    {
        try
        {
            string hashed = _passwordHasher.Hash(user.Password);
            UserCreateModel ucm = new(user.DisplayName, user.Email,  hashed); 
            return await this._userCommands.CreateUser(ucm, cancellationToken);
        }
        catch (UserDataException e)
        {
            throw new UserException("Unable to create user", e);    
        }
    }

    public async Task<AuthenticationResponse?> AuthenticateUser(AuthenticationRequest model, CancellationToken token)
    {
        try
        {
            var result = await this._userQueries.GetUserByEmail(model.Email, token);

            var (valid, upgrade) = this._passwordHasher.Check(result.Password, model.Password);

            if (valid)
            {
                return new()
                {
                    Activated = result.Activated,
                    Email = result.Email,
                    Id = result.Id,
                    DisplayName = result.DisplayName,
                    Token = GenerateJwt(result),
                    PasswordNeedsUpgrade = upgrade,
                };
            }
            else
            {
                return null;
            }
        }
        catch (UserDataException)
        {
            return default;
        }
    }

    public async Task<UserReadModel?> ValidateToken(string token, CancellationToken cancellationToken)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            Guid userId = new(jwtToken.Claims.First(x => x.Type == "id").Value);

            User user = await this._userQueries.GetUserByGuid(userId, cancellationToken);

            UserReadModel minimalUser = new()
            {
                Id = user.Id,
                Email = user.Email,
                Activated = user.Activated,
                DisplayName = user.DisplayName,
                ValidatedEmail = user.Activated,
                Roles = user.Roles.Select(r => new RoleReadModel(r.Id, r.Label)).ToImmutableArray()
            };

            return minimalUser;
        }
        catch(Exception)
        {
            return default;
        }
    }

    private string GenerateJwt(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<ImmutableArray<UserReadModel>> GetAllUsers(CancellationToken token)
    {
        try
        {
            User[] users = await this._userQueries.GetAllUsers(token);

            return users.Select(user => new UserReadModel()
            {
                Id = user.Id,
                Email = user.Email,
                Activated = user.Activated,
                DisplayName = user.DisplayName,
                ValidatedEmail = user.Activated,
                Roles = user.Roles!.Select(r => new RoleReadModel(r.Id, r.Label)).ToImmutableArray(),
            }).ToImmutableArray();
        }
        catch (UserDataException e)
        {
            throw new UserException("Unable to get users", e);    
        }
    }
}