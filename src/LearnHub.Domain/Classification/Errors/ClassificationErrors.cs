using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Classification;

public static class ClassificationErrors
{
    public static Error InvalidSlug
    => Error.Validation(code: "DomainError.Classification.InvalidSlug",
    description: "Slug must be URL-friendly");
}
