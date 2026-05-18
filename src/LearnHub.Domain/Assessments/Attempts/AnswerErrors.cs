using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Attempts;

public static class AnswerErrors
{
    public static Error QuestionIdRequired
    => Error.Validation(code: "DomainError.Answer.QuestionIdRequired",
    description: "Question id is required");

    public static Error EmptyAnswer
    => Error.Validation(code: "DomainError.Answer.EmptyAnswer",
    description: "Answer must contain a choice id or text");
}
