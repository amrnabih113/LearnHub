using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Reviews;

public static class ReviewErrors
{
    public static Error TargetIdRequired
    => Error.Validation(code: "DomainError.Review.TargetIdRequired",
    description: "Target id is required");

    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.Review.StudentIdRequired",
    description: "Student id is required");

    public static Error RatingInvalid
    => Error.Validation(code: "DomainError.Review.RatingInvalid",
    description: "Rating must be between 1 and 5");

    public static Error TitleRequired
    => Error.Validation(code: "DomainError.Review.TitleRequired",
    description: "Review title is required");

    public static Error CommentRequired
    => Error.Validation(code: "DomainError.Review.CommentRequired",
    description: "Review comment is required");

    public static Error NotDraft
    => Error.Conflict(code: "DomainError.Review.NotDraft",
    description: "Only draft reviews can be changed");

    public static Error NotPublished
    => Error.Conflict(code: "DomainError.Review.NotPublished",
    description: "Review must be published first");

    public static Error AlreadyPublished
    => Error.Conflict(code: "DomainError.Review.AlreadyPublished",
    description: "Review is already published");

    public static Error InvalidTargetType
    => Error.Validation(code: "DomainError.Review.InvalidTargetType",
    description: "Review target type is invalid");
}
