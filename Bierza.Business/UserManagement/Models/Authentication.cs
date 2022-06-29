namespace Bierza.Business.UserManagement.Models;

public class AuthenticationRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}

public class AuthenticationResponse
{
    public Guid Id { get; set; } = default;
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool Activated { get; set; }
    public string Token { get; set; } = null!;
    public bool PasswordNeedsUpgrade { get; set; } = false;
}