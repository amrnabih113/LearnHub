using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses.Sections;

public static class SectionErrors
{
    public static Error TitleRequired
        => Error.Validation(code: "DomainError.Section.TitleRequired",
            description: "Section title is required");

    public static Error DescriptionRequired
        => Error.Validation(code: "DomainError.Section.DescriptionRequired",
            description: "Section description is required");

    public static Error InvalidOrder
        => Error.Validation(code: "DomainError.Section.InvalidOrder",
            description: "Section order must be greater than zero");

    public static Error CourseIdRequired
        => Error.Validation(code: "DomainError.Section.CourseIdRequired",
            description: "Section course id is required");

    public static Error LessonsRequired
        => Error.Validation(code: "DomainError.Section.LessonsRequired",
            description: "Section lessons are required");
}