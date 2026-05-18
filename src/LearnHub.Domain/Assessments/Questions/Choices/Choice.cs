using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Questions.Choices;

public sealed class Choice : AuditableEntity
{
    public string Text { get; private set; } = default!;
    public bool IsCorrect { get; private set; }

    private Choice() { }

    private Choice(Guid id, string text, bool isCorrect) : base(id)
    {
        Text = text;
        IsCorrect = isCorrect;
    }

    public static Result<Choice> Create(Guid id, string text, bool isCorrect)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ChoiceErrors.TextRequired;
        }

        return new Choice(id, text.Trim(), isCorrect);
    }

    public Result<Updated> Update(string text, bool isCorrect)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ChoiceErrors.TextRequired;
        }

        Text = text.Trim();
        IsCorrect = isCorrect;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }
}
