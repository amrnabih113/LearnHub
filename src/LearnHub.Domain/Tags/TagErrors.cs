using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses.Tags;

public static class TagErrors
{
    public static Error NameRequired
    => Error.Validation(code: "DomainError.Tag.NameRequired",
    description: "Tag name is required");

    public static Error SlugRequired
    => Error.Validation(code: "DomainError.Tag.SlugRequired",
    description: "Tag slug is required");
}
