namespace LearnHub.Application.Common.Interfaces.Authentication;

public interface ICurrentUserService
{
    string? UserId { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }
}