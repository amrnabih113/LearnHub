using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Assessments.Events;
using LearnHub.Domain.Assessments.Grades;
using LearnHub.Domain.Assessments.ValueObjects;
using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments;

public sealed class QuizAttempt : AuditableEntity
{
    public Guid QuizId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid StudentId { get; private set; }
    public int AttemptNumber { get; private set; }
    public QuizAttemptStatus Status { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset? ExpiresAtUtc { get; private set; }
    public DateTimeOffset? SubmittedAtUtc { get; private set; }
    public decimal? ScorePercentage { get; private set; }

    public Grade? Grade { get; private set; }

    private readonly List<Attempts.Answer> _answers = [];
    public IReadOnlyCollection<Attempts.Answer> Answers => _answers.AsReadOnly();

    private QuizAttempt() { }

    private QuizAttempt(
        Guid id,
        Guid quizId,
        Guid courseId,
        Guid studentId,
        int attemptNumber,
        DateTimeOffset startedAtUtc,
        DateTimeOffset? expiresAtUtc) : base(id)
    {
        QuizId = quizId;
        CourseId = courseId;
        StudentId = studentId;
        AttemptNumber = attemptNumber;
        StartedAtUtc = startedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        Status = QuizAttemptStatus.InProgress;
    }

    public static Result<QuizAttempt> Start(
        Guid id,
        Guid quizId,
        Guid courseId,
        Guid studentId,
        int attemptNumber,
        int? timeLimitMinutes,
        int passPercentage)
    {
        if (quizId == Guid.Empty)
        {
            return QuizAttemptErrors.QuizIdRequired;
        }

        if (courseId == Guid.Empty)
        {
            return QuizAttemptErrors.CourseIdRequired;
        }

        if (studentId == Guid.Empty)
        {
            return QuizAttemptErrors.StudentIdRequired;
        }

        if (attemptNumber <= 0)
        {
            return QuizAttemptErrors.AttemptNumberInvalid;
        }

        var nowUtc = DateTimeOffset.UtcNow;
        DateTimeOffset? expiresAtUtc = timeLimitMinutes.HasValue ? nowUtc.AddMinutes(timeLimitMinutes.Value) : null;

        var attempt = new QuizAttempt(id, quizId, courseId, studentId, attemptNumber, nowUtc, expiresAtUtc);
        attempt.AddDomainEvent(new QuizStartedDomainEvent(attempt.Id, attempt.QuizId, attempt.StudentId));

        return attempt;
    }

    public Result<Updated> AnswerQuestion(Guid questionId, AnswerOption option, DateTimeOffset answeredAtUtc)
    {
        var canMutateResult = EnsureCanMutate(answeredAtUtc);
        if (canMutateResult.IsError)
        {
            return canMutateResult.Errors;
        }

        if (questionId == Guid.Empty)
        {
            return QuizAttemptErrors.QuestionIdRequired;
        }

        var existing = _answers.FirstOrDefault(a => a.QuestionId == questionId);
        if (existing is null)
        {
            var createAnswerResult = Attempts.Answer.Create(Guid.NewGuid(), questionId, option, answeredAtUtc);
            if (createAnswerResult.IsError)
            {
                return createAnswerResult.Errors;
            }

            _answers.Add(createAnswerResult.Value);
        }
        else
        {
            var updateAnswerResult = existing.Update(option, answeredAtUtc);
            if (updateAnswerResult.IsError)
            {
                return updateAnswerResult.Errors;
            }
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Submit(IReadOnlyDictionary<Guid, HashSet<Guid>> answerKey, int passPercentage, DateTimeOffset submittedAtUtc)
    {
        if (Status == QuizAttemptStatus.Submitted)
        {
            return QuizAttemptErrors.AttemptAlreadySubmitted;
        }

        var canMutateResult = EnsureCanMutate(submittedAtUtc);
        if (canMutateResult.IsError)
        {
            return canMutateResult.Errors;
        }

        var score = CalculateScore(answerKey);
        if (score.IsError)
        {
            return score.Errors;
        }

        ScorePercentage = score.Value;
        Status = QuizAttemptStatus.Submitted;
        SubmittedAtUtc = submittedAtUtc;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        var gradeResult = Grade.Create(Guid.NewGuid(), ScorePercentage.Value, passPercentage);
        if (gradeResult.IsSuccess)
        {
            Grade = gradeResult.Value;
        }

        AddDomainEvent(new QuizSubmittedDomainEvent(Id, QuizId, StudentId, ScorePercentage.Value, Grade?.IsPassed == true));
        if (Grade?.IsPassed == true)
        {
            AddDomainEvent(new QuizPassedDomainEvent(Id, QuizId, StudentId, ScorePercentage.Value));
        }

        return Result.Updated;
    }

    public Result<Updated> Submit(Quiz quiz, DateTimeOffset submittedAtUtc)
    {
        if (quiz is null)
        {
            return QuizAttemptErrors.InvalidScore;
        }

        if (Status == QuizAttemptStatus.Submitted)
        {
            return QuizAttemptErrors.AttemptAlreadySubmitted;
        }

        var canMutateResult = EnsureCanMutate(submittedAtUtc);
        if (canMutateResult.IsError)
        {
            return canMutateResult.Errors;
        }

        var score = CalculateScore(quiz);
        if (score.IsError)
        {
            return score.Errors;
        }

        ScorePercentage = score.Value;
        Status = QuizAttemptStatus.Submitted;
        SubmittedAtUtc = submittedAtUtc;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        var gradeResult = Grade.Create(Guid.NewGuid(), ScorePercentage.Value, quiz.PassingPolicy.PassPercentage);
        if (gradeResult.IsSuccess)
        {
            Grade = gradeResult.Value;
        }

        AddDomainEvent(new QuizSubmittedDomainEvent(Id, QuizId, StudentId, ScorePercentage.Value, Grade?.IsPassed == true));
        if (Grade?.IsPassed == true)
        {
            AddDomainEvent(new QuizPassedDomainEvent(Id, QuizId, StudentId, ScorePercentage.Value));
        }

        return Result.Updated;
    }

    public Result<decimal> CalculateScore(IReadOnlyDictionary<Guid, HashSet<Guid>> answerKey)
    {
        if (answerKey.Count == 0)
        {
            return QuizAttemptErrors.InvalidScore;
        }

        var answeredQuestionIds = _answers.Select(a => a.QuestionId).ToHashSet();
        var totalQuestions = answerKey.Count;
        var correctCount = 0;

        foreach (var item in answerKey)
        {
            if (!answeredQuestionIds.Contains(item.Key))
            {
                continue;
            }

            var answer = _answers.First(a => a.QuestionId == item.Key);
            if (answer.SelectedChoiceId.HasValue && item.Value.Contains(answer.SelectedChoiceId.Value))
            {
                correctCount++;
            }
        }

        var score = totalQuestions == 0
            ? 0m
            : decimal.Round((correctCount * 100m) / totalQuestions, 2);

        return Math.Clamp(score, 0m, 100m);
    }

    public Result<decimal> CalculateScore(Quiz quiz)
    {
        if (quiz is null)
        {
            return QuizAttemptErrors.InvalidScore;
        }

        if (quiz.Questions.Count == 0)
        {
            return QuizAttemptErrors.InvalidScore;
        }

        var totalPoints = quiz.TotalPoints();
        if (totalPoints <= 0)
        {
            return QuizAttemptErrors.InvalidScore;
        }

        var earnedPoints = 0m;

        foreach (var question in quiz.Questions)
        {
            var answer = _answers.FirstOrDefault(item => item.QuestionId == question.Id);
            if (answer is null)
            {
                continue;
            }

            var isCorrect = question.Type switch
            {
                QuestionType.MultipleChoice or QuestionType.TrueFalse =>
                    answer.SelectedChoiceId.HasValue && question.Choices.Any(choice => choice.Id == answer.SelectedChoiceId.Value && choice.IsCorrect),
                QuestionType.ShortAnswer =>
                    !string.IsNullOrWhiteSpace(answer.TextAnswer) &&
                    !string.IsNullOrWhiteSpace(question.CorrectTextAnswer) &&
                    NormalizeText(answer.TextAnswer) == NormalizeText(question.CorrectTextAnswer),
                _ => false
            };

            if (isCorrect)
            {
                earnedPoints += question.Points;
            }
        }

        var score = decimal.Round((earnedPoints * 100m) / totalPoints, 2);
        return Math.Clamp(score, 0m, 100m);
    }

    public Result<Updated> MarkTimedOut(DateTimeOffset nowUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            return Result.Updated;
        }

        if (ExpiresAtUtc.HasValue && nowUtc >= ExpiresAtUtc.Value)
        {
            Status = QuizAttemptStatus.TimedOut;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        return Result.Updated;
    }

    public Result<Updated> Cancel(DateTimeOffset nowUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            return QuizAttemptErrors.AttemptClosed;
        }

        Status = QuizAttemptStatus.Canceled;
        UpdatedAtUtc = nowUtc;
        return Result.Updated;
    }

    private Result<Updated> EnsureCanMutate(DateTimeOffset nowUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
        {
            return QuizAttemptErrors.AttemptClosed;
        }

        if (ExpiresAtUtc.HasValue && nowUtc >= ExpiresAtUtc.Value)
        {
            Status = QuizAttemptStatus.TimedOut;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
            return QuizAttemptErrors.TimeoutReached;
        }

        return Result.Updated;
    }

    private static string NormalizeText(string value)
    {
        return string.Join(" ", value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
