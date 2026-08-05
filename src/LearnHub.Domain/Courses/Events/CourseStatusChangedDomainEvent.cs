using LearnHub.Domain.Common;
using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Domain.Courses.Events;

public sealed class CourseStatusChangedDomainEvent : DomainEvent
{
    public CourseStatusChangedDomainEvent(Guid courseId, CourseStatus oldStatus, CourseStatus newStatus)
    {
        CourseId = courseId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public Guid CourseId { get; }
    public CourseStatus OldStatus { get; }
    public CourseStatus NewStatus { get; }

    public CourseStatus PreviousStatus => OldStatus;

    public CourseStatus CurrentStatus => NewStatus;
}