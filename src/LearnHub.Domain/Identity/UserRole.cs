namespace LearnHub.Domain.Identity;

public sealed class UserRole
{
    public Guid UserId { get; private set; }

    public Role Role { get; private set; }

    public User User { get; private set; } = default!;

    private UserRole()
    {
    }

    private UserRole(Guid userId, Role role)
    {
        UserId = userId;
        Role = role;
    }

    public static UserRole Create(Guid userId, Role role)
    {
        return new UserRole(userId, role);
    }
}