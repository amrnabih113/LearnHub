using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Enrollments.LessonProgress;

public static class LessonProgressErrors
{
    public static Error EnrollmentIdRequired
    => Error.Validation(code: "DomainError.LessonProgress.EnrollmentIdRequired",
    description: "Enrollment id is required");

    public static Error LessonIdRequired
    => Error.Validation(code: "DomainError.LessonProgress.LessonIdRequired",
    description: "Lesson id is required");

    public static Error InvalidWatchDuration
    => Error.Validation(code: "DomainError.LessonProgress.InvalidWatchDuration",
    description: "Watch duration cannot be negative");
}
