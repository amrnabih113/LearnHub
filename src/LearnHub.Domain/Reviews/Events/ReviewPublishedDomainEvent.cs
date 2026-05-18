using LearnHub.Domain.Common;

namespace LearnHub.Domain.Reviews.Events;

public sealed class ReviewPublishedDomainEvent : DomainEvent
{
    public ReviewPublishedDomainEvent(Guid reviewId, string targetType)
    {
        ReviewId = reviewId;
        TargetType = targetType;
    }

    public Guid ReviewId { get; }
    public string TargetType { get; }
}
