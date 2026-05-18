using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.ValueObjects;

public sealed record AnswerOption
{
    public Guid ChoiceId { get; }
    public string? TextAnswer { get; }

    private AnswerOption(Guid choiceId, string? textAnswer)
    {
        ChoiceId = choiceId;
        TextAnswer = textAnswer;
    }

    public static Result<AnswerOption> FromChoice(Guid choiceId)
    {
        if (choiceId == Guid.Empty)
        {
            return QuizAttemptErrors.ChoiceIdRequired;
        }

        return new AnswerOption(choiceId, null);
    }

    public static Result<AnswerOption> FromText(string textAnswer)
    {
        if (string.IsNullOrWhiteSpace(textAnswer))
        {
            return QuizAttemptErrors.AnswerTextRequired;
        }

        return new AnswerOption(Guid.Empty, textAnswer.Trim());
    }
}
