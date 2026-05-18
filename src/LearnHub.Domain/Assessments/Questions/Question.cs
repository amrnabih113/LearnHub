using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Assessments.Questions.Choices;
using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Questions;

public sealed class Question : AuditableEntity
{
    public string Prompt { get; private set; } = default!;
    public QuestionType Type { get; private set; }
    public int Points { get; private set; }
    public int Order { get; private set; }
    public string? CorrectTextAnswer { get; private set; }

    private readonly List<Choice> _choices = [];
    public IReadOnlyCollection<Choice> Choices => _choices.AsReadOnly();

    private Question() { }

    private Question(Guid id, string prompt, QuestionType type, int points, int order) : base(id)
    {
        Prompt = prompt;
        Type = type;
        Points = points;
        Order = order;
    }

    public static Result<Question> Create(Guid id, string prompt, QuestionType type, int points, int order)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return QuestionErrors.PromptRequired;
        }

        if (points <= 0)
        {
            return QuestionErrors.PointsInvalid;
        }

        return new Question(id, prompt.Trim(), type, points, order);
    }

    public Result<Updated> Update(string prompt, int points, int order)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return QuestionErrors.PromptRequired;
        }

        if (points <= 0)
        {
            return QuestionErrors.PointsInvalid;
        }

        Prompt = prompt.Trim();
        Points = points;
        Order = order;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> AddChoice(Guid choiceId, string text, bool isCorrect)
    {
        if (Type == QuestionType.ShortAnswer)
        {
            return Error.Conflict(
                code: "DomainError.Question.ChoiceNotAllowed",
                description: "Short answer questions do not support choices");
        }

        var createChoiceResult = Choice.Create(choiceId, text, isCorrect);
        if (createChoiceResult.IsError)
        {
            return createChoiceResult.Errors;
        }

        _choices.Add(createChoiceResult.Value);
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> SetCorrectTextAnswer(string correctTextAnswer)
    {
        if (Type != QuestionType.ShortAnswer)
        {
            return QuestionErrors.TextAnswerNotAllowed;
        }

        if (string.IsNullOrWhiteSpace(correctTextAnswer))
        {
            return QuestionErrors.CorrectTextAnswerRequired;
        }

        CorrectTextAnswer = correctTextAnswer.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> EnsurePublishable()
    {
        if (Type is QuestionType.MultipleChoice or QuestionType.TrueFalse)
        {
            if (_choices.Count == 0)
            {
                return QuestionErrors.ChoicesRequired;
            }

            if (_choices.All(c => !c.IsCorrect))
            {
                return QuestionErrors.CorrectChoiceRequired;
            }
        }
        else if (Type == QuestionType.ShortAnswer)
        {
            if (string.IsNullOrWhiteSpace(CorrectTextAnswer))
            {
                return QuestionErrors.CorrectTextAnswerRequired;
            }
        }

        return Result.Updated;
    }
}
