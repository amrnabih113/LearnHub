using LearnHub.Domain.Common;

namespace LearnHub.Domain.Reviews.Events;

public sealed class InstructorReviewCreatedDomainEvent : DomainEvent
{
    public InstructorReviewCreatedDomainEvent(Guid reviewId, Guid instructorId, Guid studentId)
    {
        ReviewId = reviewId;
        InstructorId = instructorId;
        StudentId = studentId;
    }

    public Guid ReviewId { get; }
    public Guid InstructorId { get; }
    public Guid StudentId { get; }
}
