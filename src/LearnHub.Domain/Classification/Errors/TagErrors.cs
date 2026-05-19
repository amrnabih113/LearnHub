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

    public static Error NotActive
    => Error.Conflict(code: "DomainError.Tag.NotActive",
    description: "Only active tags can be changed");

    public static Error AlreadyArchived
    => Error.Conflict(code: "DomainError.Tag.AlreadyArchived",
    description: "Tag is already archived");
}
