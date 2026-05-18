using LearnHub.Domain.Common;

namespace LearnHub.Domain.Reviews.Events;

public sealed class InstructorReviewCreatedDomainEvent : DomainEvent
{
    public InstructorReviewCreatedDomainEvent(Guid reviewId, string instructorId, string studentId)
    {
        ReviewId = reviewId;
        InstructorId = instructorId;
        StudentId = studentId;
    }

    public Guid ReviewId { get; }
    public string InstructorId { get; }
    public string StudentId { get; }
}
