using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Classification;

public static class TagErrors
{
    public static Error NameRequired
    => Error.Validation(code: "DomainError.Tag.NameRequired",
    description: "Tag name is required");

    public static Error SlugRequired
    => Error.Validation(code: "DomainError.Tag.SlugRequired",
    description: "Tag slug is required");

    public static Error TagNotFound
    => Error.NotFound(code: "DomainError.Tag.TagNotFound",
    description: "Tag was not found");

    public static Error DuplicateName
    => Error.Conflict(code: "DomainError.Tag.DuplicateName",
    description: "A tag with this name or slug already exists");
}
