using LearnHub.Domain.Assessments.ValueObjects;
using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Attempts;

public sealed class Answer : AuditableEntity
{
    public Guid QuestionId { get; private set; }
    public Guid? SelectedChoiceId { get; private set; }
    public string? TextAnswer { get; private set; }
    public DateTimeOffset AnsweredAtUtc { get; private set; }

    private Answer() { }

    private Answer(Guid id, Guid questionId, Guid? selectedChoiceId, string? textAnswer, DateTimeOffset answeredAtUtc) : base(id)
    {
        QuestionId = questionId;
        SelectedChoiceId = selectedChoiceId;
        TextAnswer = textAnswer;
        AnsweredAtUtc = answeredAtUtc;
    }

    public static Result<Answer> Create(Guid id, Guid questionId, AnswerOption option, DateTimeOffset answeredAtUtc)
    {
        if (questionId == Guid.Empty)
        {
            return AnswerErrors.QuestionIdRequired;
        }

        if (option.ChoiceId == Guid.Empty && string.IsNullOrWhiteSpace(option.TextAnswer))
        {
            return AnswerErrors.EmptyAnswer;
        }

        return new Answer(
            id,
            questionId,
            option.ChoiceId == Guid.Empty ? null : option.ChoiceId,
            option.TextAnswer,
            answeredAtUtc);
    }

    public Result<Updated> Update(AnswerOption option, DateTimeOffset answeredAtUtc)
    {
        if (option.ChoiceId == Guid.Empty && string.IsNullOrWhiteSpace(option.TextAnswer))
        {
            return AnswerErrors.EmptyAnswer;
        }

        SelectedChoiceId = option.ChoiceId == Guid.Empty ? null : option.ChoiceId;
        TextAnswer = option.TextAnswer;
        AnsweredAtUtc = answeredAtUtc;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }
}
