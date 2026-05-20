using LearnHub.Domain.Common;

namespace LearnHub.Domain.Enrollments.Events;


public sealed class CourseCompletedDomainEvent : DomainEvent
{
    public CourseCompletedDomainEvent(Guid enrollmentId, Guid courseId, Guid studentId)
    {
        EnrollmentId = enrollmentId;
        CourseId = courseId;
        StudentId = studentId;
    }

    public Guid EnrollmentId { get; }
    public Guid CourseId { get; }
    public Guid StudentId { get; }
}