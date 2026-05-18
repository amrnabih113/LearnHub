using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Questions;

public static class QuestionErrors
{
    public static Error PromptRequired
    => Error.Validation(code: "DomainError.Question.PromptRequired",
    description: "Question prompt is required");

    public static Error PointsInvalid
    => Error.Validation(code: "DomainError.Question.PointsInvalid",
    description: "Question points must be greater than zero");

    public static Error ChoicesRequired
    => Error.Validation(code: "DomainError.Question.ChoicesRequired",
    description: "MCQ and True/False questions must contain choices");

    public static Error CorrectChoiceRequired
    => Error.Validation(code: "DomainError.Question.CorrectChoiceRequired",
    description: "At least one correct choice is required");

    public static Error CorrectTextAnswerRequired
    => Error.Validation(code: "DomainError.Question.CorrectTextAnswerRequired",
    description: "Short answer questions must define a correct text answer");

    public static Error TextAnswerNotAllowed
    => Error.Conflict(code: "DomainError.Question.TextAnswerNotAllowed",
    description: "Only short answer questions can define a correct text answer");
}
