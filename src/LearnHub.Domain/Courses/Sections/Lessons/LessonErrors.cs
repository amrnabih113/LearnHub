using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses.Sections.Lessons;

public static class LessonErrors
{
    public static Error TitleRequired
        => Error.Validation(code: "DomainError.Lesson.TitleRequired",
            description: "Lesson title is required");

    public static Error DescriptionRequired
        => Error.Validation(code: "DomainError.Lesson.DescriptionRequired",
            description: "Lesson description is required");

    public static Error VideoUrlRequired
        => Error.Validation(code: "DomainError.Lesson.VideoUrlRequired",
            description: "Lesson video url is required");

    public static Error ContentRequired
        => Error.Validation(code: "DomainError.Lesson.ContentRequired",
            description: "Lesson content is required");

    public static Error InvalidDuration
        => Error.Validation(code: "DomainError.Lesson.InvalidDuration",
            description: "Lesson duration must be greater than zero");

    public static Error InvalidOrder
        => Error.Validation(code: "DomainError.Lesson.InvalidOrder",
            description: "Lesson order must be greater than zero");

    public static Error SectionIdRequired
        => Error.Validation(code: "DomainError.Lesson.SectionIdRequired",
            description: "Lesson section id is required");

    public static Error ResourcesRequired
        => Error.Validation(code: "DomainError.Lesson.ResourcesRequired",
            description: "Lesson resources are required");
}