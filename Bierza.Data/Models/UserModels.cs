namespace Bierza.Data.Models;

public record UserBaseModel(string UserName, string Role);

public record UserReadModel(Guid Id, string UserName, string Role) : UserBaseModel(UserName, Role);

public record UserCreateModel(string UserName, string Role, string Password) : UserBaseModel(UserName, Role);

public record UserUpdateModel(Guid Id, string UserName, string Role) : UserBaseModel(UserName, Role);

public record UserPasswordUpdateModel(Guid Id, string Password);

public record ValidateUserModel(string UserName, string Password);

public record UserQueryModel(params string[] UserNames);
public record UserQueryByRolesModel(params string[] Roles);

public record UserValidationModel(bool Valid, bool NeedsUpgrade);
