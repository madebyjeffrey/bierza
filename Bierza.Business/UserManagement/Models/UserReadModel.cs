namespace Bierza.Business.UserManagement.Models;

public record RoleReadModel(Guid Id, string Label);

public class UserReadModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool ValidatedEmail { get; set; } = false;
    public bool Activated { get; set; } = false;
    public IReadOnlyCollection<RoleReadModel> Roles { get; set; } = null!;
}