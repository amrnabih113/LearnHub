using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Enrollments;

namespace LearnHub.Application.Features.Enrollments.Mappers;

public static class EnrollmentMappingExtensions
{
    public static EnrollmentDto ToDto(this Enrollment enrollment)
    {
        return new EnrollmentDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.Student != null ? $"{enrollment.Student.FirstName} {enrollment.Student.LastName}" : null,
            enrollment.CourseId,
            enrollment.Course != null ? enrollment.Course.Title : null,
            enrollment.Status,
            enrollment.ProgressPercentage,
            enrollment.CompletedAtUtc,
            enrollment.CreatedAtUtc);
    }

    public static EnrollmentDetailsDto ToDetailsDto(this Enrollment enrollment)
    {
        return new EnrollmentDetailsDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.Student != null ? $"{enrollment.Student.FirstName} {enrollment.Student.LastName}" : null,
            enrollment.CourseId,
            enrollment.Course != null ? enrollment.Course.Title : null,
            enrollment.Status,
            enrollment.ProgressPercentage,
            enrollment.CompletedAtUtc,
            enrollment.CreatedAtUtc,
            enrollment.Certificate != null
                ? new CertificateDto(
                    enrollment.Certificate.Id,
                    enrollment.Certificate.EnrollmentId,
                    enrollment.Certificate.StudentId,
                    enrollment.Certificate.Code,

                    enrollment.Certificate.IssuedAtUtc)
                : null,
            enrollment.LessonsProgress
                .Select(lp => new LessonProgressDto(
                    lp.Id,
                    lp.LessonId,
                    lp.WatchDurationSeconds,
                    lp.IsCompleted,
                    lp.CompletedAtUtc,
                    lp.CreatedAtUtc))
                .ToList());
    }
}
