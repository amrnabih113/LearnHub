using LearnHub.Domain.Common;

namespace LearnHub.Domain.Identity.Events;

public sealed class UserCreatedDomainEvent(
    Guid userId,
    string email) : DomainEvent
{
    public Guid UserId { get; } = userId;

    public string Email { get; } = email;
}