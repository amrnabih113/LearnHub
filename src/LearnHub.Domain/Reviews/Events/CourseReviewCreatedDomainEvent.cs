using LearnHub.Domain.Common;

namespace LearnHub.Domain.Reviews.Events;

public sealed class CourseReviewCreatedDomainEvent : DomainEvent
{
    public CourseReviewCreatedDomainEvent(Guid reviewId, Guid courseId, Guid studentId)
    {
        ReviewId = reviewId;
        CourseId = courseId;
        StudentId = studentId;
    }

    public Guid ReviewId { get; }
    public Guid CourseId { get; }
    public Guid StudentId { get; }
}
