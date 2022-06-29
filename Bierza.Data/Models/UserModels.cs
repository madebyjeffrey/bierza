namespace Bierza.Data.Models;

public record UserBaseModel(string DisplayName, string Email, bool Activated);

public record UserCreateModel(string DisplayName, string Email, string Password) 
    : UserBaseModel(DisplayName, Email, false);

