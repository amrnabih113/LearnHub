using LearnHub.Domain.Common;

namespace LearnHub.Domain.Identity.Events;

public class UserCreatedDomainEvent : DomainEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;   
}