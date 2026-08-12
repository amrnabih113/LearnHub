using LearnHub.Domain.Common;

namespace LearnHub.Domain.Enrollments.Events;

public sealed class EnrollmentCreatedDomainEvent : DomainEvent
{
    public EnrollmentCreatedDomainEvent(Guid enrollmentId, Guid studentId, Guid courseId)
    {
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        CourseId = courseId;
    }

    public Guid EnrollmentId { get; }
    public Guid StudentId { get; }
    public Guid CourseId { get; }
}
