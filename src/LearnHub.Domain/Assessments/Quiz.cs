using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Assessments.Events;
using LearnHub.Domain.Assessments.Questions;
using LearnHub.Domain.Assessments.Questions.Choices;
using LearnHub.Domain.Assessments.ValueObjects;
using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments;

public sealed class Quiz : AuditableEntity
{
    public Guid CourseId { get; private set; }
    public Guid? SectionId { get; private set; }
    public QuizType Type { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public QuizStatus Status { get; private set; }
    public int? TimeLimitMinutes { get; private set; }
    public PassingPolicy PassingPolicy { get; private set; } = default!;

    private readonly List<Question> _questions = [];
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    private Quiz() { }

    private Quiz(Guid id, Guid courseId, Guid? sectionId, QuizType type, string title, string? description, int? timeLimitMinutes, PassingPolicy passingPolicy) : base(id)
    {
        CourseId = courseId;
        SectionId = sectionId;
        Type = type;
        Title = title;
        Description = description;
        TimeLimitMinutes = timeLimitMinutes;
        PassingPolicy = passingPolicy;
        Status = QuizStatus.Draft;
    }

    public static Result<Quiz> Create(Guid id, Guid courseId, string title, string? description, int? timeLimitMinutes, int maxAttempts, int passPercentage)
    {
        return CreateSectionQuiz(id, courseId, null, title, description, timeLimitMinutes, maxAttempts, passPercentage);
    }

    public static Result<Quiz> CreateSectionQuiz(Guid id, Guid courseId, Guid? sectionId, string title, string? description, int? timeLimitMinutes, int maxAttempts, int passPercentage)
    {
        if (courseId == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return QuizErrors.TitleRequired;
        }

        if (timeLimitMinutes is <= 0)
        {
            return QuizErrors.TimeLimitInvalid;
        }

        var passingPolicyResult = PassingPolicy.Create(maxAttempts, passPercentage);
        if (passingPolicyResult.IsError)
        {
            return passingPolicyResult.Errors;
        }

        var quiz = new Quiz(id, courseId, sectionId, QuizType.Section, title.Trim(), description?.Trim(), timeLimitMinutes, passingPolicyResult.Value);
        quiz.AddDomainEvent(new QuizCreatedDomainEvent(quiz.Id, quiz.CourseId));

        return quiz;
    }

    public static Result<Quiz> CreateFinalExam(Guid id, Guid courseId, string title, string? description, int? timeLimitMinutes, int maxAttempts, int passPercentage)
    {
        if (courseId == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return QuizErrors.TitleRequired;
        }

        if (timeLimitMinutes is <= 0)
        {
            return QuizErrors.TimeLimitInvalid;
        }

        var passingPolicyResult = PassingPolicy.Create(maxAttempts, passPercentage);
        if (passingPolicyResult.IsError)
        {
            return passingPolicyResult.Errors;
        }

        var quiz = new Quiz(id, courseId, null, QuizType.Final, title.Trim(), description?.Trim(), timeLimitMinutes, passingPolicyResult.Value);
        quiz.AddDomainEvent(new QuizCreatedDomainEvent(quiz.Id, quiz.CourseId));

        return quiz;
    }

    public Result<Updated> UpdateDetails(string title, string? description, int? timeLimitMinutes, int maxAttempts, int passPercentage)
    {
        if (Status != QuizStatus.Draft)
        {
            return QuizErrors.NotDraft;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return QuizErrors.TitleRequired;
        }

        if (timeLimitMinutes is <= 0)
        {
            return QuizErrors.TimeLimitInvalid;
        }

        var passingPolicyResult = PassingPolicy.Create(maxAttempts, passPercentage);
        if (passingPolicyResult.IsError)
        {
            return passingPolicyResult.Errors;
        }

        Title = title.Trim();
        Description = description?.Trim();
        TimeLimitMinutes = timeLimitMinutes;
        PassingPolicy = passingPolicyResult.Value;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> AddQuestion(Guid questionId, string prompt, QuestionType type, int points, int order)
    {
        if (Status != QuizStatus.Draft)
        {
            return QuizErrors.NotDraft;
        }

        var createResult = Question.Create(questionId, prompt, type, points, order);
        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        _questions.Add(createResult.Value);
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> SetCorrectTextAnswer(Guid questionId, string correctTextAnswer)
    {
        if (Status != QuizStatus.Draft)
        {
            return QuizErrors.NotDraft;
        }

        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        var updateResult = question.SetCorrectTextAnswer(correctTextAnswer);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> AddChoice(Guid questionId, Guid choiceId, string text, bool isCorrect)
    {
        if (Status != QuizStatus.Draft)
        {
            return QuizErrors.NotDraft;
        }

        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        var addChoiceResult = question.AddChoice(choiceId, text, isCorrect);
        if (addChoiceResult.IsError)
        {
            return addChoiceResult.Errors;
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Publish()
    {
        if (Status != QuizStatus.Draft)
        {
            return QuizErrors.NotDraft;
        }

        if (_questions.Count == 0)
        {
            return QuizErrors.QuestionsRequired;
        }

        foreach (var question in _questions)
        {
            var publishableResult = question.EnsurePublishable();
            if (publishableResult.IsError)
            {
                return publishableResult.Errors;
            }
        }

        Status = QuizStatus.Published;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new QuizPublishedDomainEvent(Id, CourseId));

        return Result.Updated;
    }

    public Result<Updated> Archive()
    {
        Status = QuizStatus.Archived;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<QuizAttempt> StartAttempt(Guid attemptId, Guid studentId, int previousAttemptsCount)
    {
        if (Status != QuizStatus.Published)
        {
            return QuizErrors.NotPublished;
        }

        if (previousAttemptsCount >= PassingPolicy.MaxAttempts)
        {
            return QuizErrors.MaxAttemptsExceeded;
        }

        var attemptNumber = previousAttemptsCount + 1;
        return QuizAttempt.Start(
     attemptId,
     Id,
     CourseId,
     studentId,
     attemptNumber,
     TimeLimitMinutes);
    }

    public IReadOnlyDictionary<Guid, HashSet<Guid>> BuildAnswerKey()
    {
        return _questions.ToDictionary(
            q => q.Id,
            q => q.Choices.Where(c => c.IsCorrect).Select(c => c.Id).ToHashSet());
    }

    public bool CanAutoScore()
    {
        return _questions.All(q => q.Type is QuestionType.MultipleChoice or QuestionType.TrueFalse);
    }

    public int TotalPoints()
    {
        return _questions.Sum(q => q.Points);
    }
}
