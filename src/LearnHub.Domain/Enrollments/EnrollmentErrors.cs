using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Enrollments;

public static class EnrollmentErrors
{
    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.Enrollment.StudentIdRequired",
    description: "Student id is required");

    public static Error CourseIdRequired
    => Error.Validation(code: "DomainError.Enrollment.CourseIdRequired",
    description: "Course id is required");

    public static Error LessonIdRequired
    => Error.Validation(code: "DomainError.Enrollment.LessonIdRequired",
    description: "Lesson id is required");

    public static Error TotalLessonsInvalid
    => Error.Validation(code: "DomainError.Enrollment.TotalLessonsInvalid",
    description: "Total lessons must be greater than zero");

    public static Error InvalidWatchDuration
    => Error.Validation(code: "DomainError.Enrollment.InvalidWatchDuration",
    description: "Watch duration cannot be negative");

    public static Error InvalidEnrollmentId
    => Error.Validation(code: "DomainError.Enrollment.InvalidEnrollmentId",
    description: "Enrollment id is required");

    public static Error NotActive
    => Error.Conflict(code: "DomainError.Enrollment.NotActive",
    description: "Only active enrollments can be updated");

    public static Error Dropped
    => Error.Conflict(code: "DomainError.Enrollment.Dropped",
    description: "Dropped enrollments cannot be completed");

    public static Error AlreadyCompleted
    => Error.Conflict(code: "DomainError.Enrollment.AlreadyCompleted",
    description: "Completed enrollments cannot be canceled");
}
