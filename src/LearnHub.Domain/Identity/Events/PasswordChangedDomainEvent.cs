using LearnHub.Domain.Common;

namespace LearnHub.Domain.Identity.Events;


public sealed class PasswordChangedDomainEvent(Guid userId) : DomainEvent
{
    public Guid UserId { get; } = userId;
}