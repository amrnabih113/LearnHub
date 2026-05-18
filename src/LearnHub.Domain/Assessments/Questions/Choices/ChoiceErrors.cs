using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Questions.Choices;

public static class ChoiceErrors
{
    public static Error TextRequired
    => Error.Validation(code: "DomainError.Choice.TextRequired",
    description: "Choice text is required");
}
