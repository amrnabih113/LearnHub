using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses.Sections.Lessons.Resources;

public static class ResourceErrors
{
    public static Error LessonIdRequired
        => Error.Validation(code: "DomainError.Resource.LessonIdRequired",
            description: "Resource must be associated with a lesson");
    public static Error NameRequired
        => Error.Validation(code: "DomainError.Resource.NameRequired",
            description: "Resource name is required");

    public static Error UrlRequired
        => Error.Validation(code: "DomainError.Resource.UrlRequired",
            description: "Resource url is required");

    public static Error InvalidResourceType
        => Error.Validation(code: "DomainError.Resource.InvalidResourceType",
            description: "Resource type is invalid");

    public static Error InvalidSize
        => Error.Validation(code: "DomainError.Resource.InvalidSize",
            description: "Resource size must be zero or greater");
}