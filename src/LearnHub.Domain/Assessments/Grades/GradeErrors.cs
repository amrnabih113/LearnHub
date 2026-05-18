using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Grades;

public static class GradeErrors
{
    public static Error InvalidScore
    => Error.Validation(code: "DomainError.Grade.InvalidScore",
    description: "Score percentage must be between 0 and 100");
}
