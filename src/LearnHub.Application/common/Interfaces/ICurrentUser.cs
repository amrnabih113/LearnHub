using LearnHub.Domain.Identity;

namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Role? Role { get; }
    string? Email { get; }

    bool IsAuthenticated { get; }
}