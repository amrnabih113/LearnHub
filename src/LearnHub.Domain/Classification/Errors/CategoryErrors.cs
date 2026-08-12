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

    public static Error CategoryNotFound
    => Error.NotFound(code: "DomainError.Category.CategoryNotFound",
    description: "Category was not found");

    public static Error DuplicateName
    => Error.Conflict(code: "DomainError.Category.DuplicateName",
    description: "A category with this name or slug already exists");

    public static Error CategoryHasSubcategories
    => Error.Conflict(code: "DomainError.Category.HasSubcategories",
    description: "Cannot delete category that contains subcategories");

    public static Error CategoryHasCourses
    => Error.Conflict(code: "DomainError.Category.HasCourses",
    description: "Cannot delete category that contains assigned courses");
}
