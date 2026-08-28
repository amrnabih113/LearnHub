using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.LearningPaths;

public static class LearningPathErrors
{
    public static Error NotFound
        => Error.NotFound("LearningPath.NotFound", "Learning path was not found.");

    public static Error TitleRequired
        => Error.Validation("LearningPath.TitleRequired", "Title is required.");

    public static Error DescriptionRequired
        => Error.Validation("LearningPath.DescriptionRequired", "Description is required.");

    public static Error InvalidStatus
        => Error.Validation("LearningPath.InvalidStatus", "Invalid learning path status.");

    public static Error CourseRequired
        => Error.Validation("LearningPath.CourseRequired", "Learning path must contain at least one course before publishing.");

    public static Error CourseAlreadyInPath
        => Error.Conflict("LearningPath.CourseAlreadyInPath", "This course is already added to the learning path.");

    public static Error CourseNotInPath
        => Error.NotFound("LearningPath.CourseNotInPath", "Course is not present in this learning path.");

    public static Error InvalidOrder
        => Error.Validation("LearningPath.InvalidOrder", "Invalid course ordering provided.");

    public static Error Unauthorized
        => Error.Forbidden("LearningPath.Unauthorized", "You are not authorized to modify this learning path.");
}
