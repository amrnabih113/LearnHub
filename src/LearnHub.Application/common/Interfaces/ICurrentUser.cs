using LearnHub.Domain.Identity;

namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    IReadOnlyCollection<Role> Roles { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(Role role);
}