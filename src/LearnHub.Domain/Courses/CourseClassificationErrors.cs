using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Courses;

public static class CourseClassificationErrors
{
    public static Error CategoryIdRequired
    => Error.Validation(code: "DomainError.Course.CategoryIdRequired",
    description: "Category id is required");

    public static Error TagIdRequired
    => Error.Validation(code: "DomainError.Course.TagIdRequired",
    description: "Tag id is required");

    public static Error TagAlreadyAssigned
    => Error.Conflict(code: "DomainError.Course.TagAlreadyAssigned",
    description: "Tag is already assigned to the course");

    public static Error TagLimitReached
    => Error.Conflict(code: "DomainError.Course.TagLimitReached",
    description: "Maximum tag count for the course has been reached");

    public static Error TagNotFound
    => Error.NotFound(code: "DomainError.Course.TagNotFound",
    description: "Tag was not found on the course");
}
