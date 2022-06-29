namespace Bierza.Business.UserManagement.Models;

public class CreateUserRequestModel
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}