using LearnHub.Domain.Common;
using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Domain.Courses.Events;

public sealed class CourseStatusChangedDomainEvent : DomainEvent
{
    public CourseStatusChangedDomainEvent(Guid courseId, CourseStatus previousStatus, CourseStatus currentStatus)
    {
        CourseId = courseId;
        PreviousStatus = previousStatus;
        CurrentStatus = currentStatus;
    }

    public Guid CourseId { get; }
    public CourseStatus PreviousStatus { get; }
    public CourseStatus CurrentStatus { get; }
}