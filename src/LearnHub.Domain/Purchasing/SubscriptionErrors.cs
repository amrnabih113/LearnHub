using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing;

public static class SubscriptionErrors
{
    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.Subscription.StudentIdRequired",
    description: "Student id is required");

    public static Error CourseIdRequired
    => Error.Validation(code: "DomainError.Subscription.CourseIdRequired",
    description: "Course id is required");

    public static Error ExpirationRequired
    => Error.Validation(code: "DomainError.Subscription.ExpirationRequired",
    description: "Subscription expiration is required");
}
