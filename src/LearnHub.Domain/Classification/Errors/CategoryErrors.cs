using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Classification;

public static class CategoryErrors
{
    public static Error NameRequired
    => Error.Validation(code: "DomainError.Category.NameRequired",
    description: "Category name is required");

    public static Error SlugRequired
    => Error.Validation(code: "DomainError.Category.SlugRequired",
    description: "Category slug is required");

    public static Error ParentCategoryRequired
    => Error.Validation(code: "DomainError.Category.ParentCategoryRequired",
    description: "Parent category id is invalid");

    public static Error HierarchyInvalid
    => Error.Conflict(code: "DomainError.Category.HierarchyInvalid",
    description: "Category hierarchy is invalid");

    public static Error NotActive
    => Error.Conflict(code: "DomainError.Category.NotActive",
    description: "Only active categories can be changed");

    public static Error AlreadyArchived
    => Error.Conflict(code: "DomainError.Category.AlreadyArchived",
    description: "Category is already archived");
}
