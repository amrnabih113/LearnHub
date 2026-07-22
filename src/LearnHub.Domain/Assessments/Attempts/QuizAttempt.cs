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
    public IReadOnlyCollection<Attempts.Answer> Answers =>
        _answers.AsReadOnly();



    private QuizAttempt() { }



    private QuizAttempt(
        Guid id,
        Guid quizId,
        Guid courseId,
        Guid studentId,
        int attemptNumber,
        DateTimeOffset startedAtUtc,
        DateTimeOffset? expiresAtUtc)
        : base(id)
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
        int? timeLimitMinutes)
    {
        if (quizId == Guid.Empty)
            return QuizAttemptErrors.QuizIdRequired;


        if (courseId == Guid.Empty)
            return QuizAttemptErrors.CourseIdRequired;


        if (studentId == Guid.Empty)
            return QuizAttemptErrors.StudentIdRequired;


        if (attemptNumber <= 0)
            return QuizAttemptErrors.AttemptNumberInvalid;


        var nowUtc = DateTimeOffset.UtcNow;

        DateTimeOffset? expiresAtUtc =
            timeLimitMinutes.HasValue
                ? nowUtc.AddMinutes(timeLimitMinutes.Value)
                : null;


        var attempt = new QuizAttempt(
            id,
            quizId,
            courseId,
            studentId,
            attemptNumber,
            nowUtc,
            expiresAtUtc);


        attempt.AddDomainEvent(
            new QuizStartedDomainEvent(
                attempt.Id,
                attempt.QuizId,
                attempt.StudentId));


        return attempt;
    }



    public Result<Updated> AnswerQuestion(
        Guid questionId,
        AnswerOption option,
        DateTimeOffset answeredAtUtc)
    {
        var canMutate = EnsureCanMutate(answeredAtUtc);

        if (canMutate.IsError)
            return canMutate.Errors;


        if (questionId == Guid.Empty)
            return QuizAttemptErrors.QuestionIdRequired;



        var existing =
            _answers.FirstOrDefault(
                a => a.QuestionId == questionId);



        if (existing is null)
        {
            var answerResult =
                Attempts.Answer.Create(
                    Guid.NewGuid(),
                    questionId,
                    option,
                    answeredAtUtc);


            if (answerResult.IsError)
                return answerResult.Errors;


            _answers.Add(answerResult.Value);
        }
        else
        {
            var updateResult =
                existing.Update(
                    option,
                    answeredAtUtc);


            if (updateResult.IsError)
                return updateResult.Errors;
        }


        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }



    public Result<Updated> Submit(
        IReadOnlyDictionary<Guid, HashSet<Guid>> answerKey,
        int passPercentage,
        DateTimeOffset submittedAtUtc)
    {
        if (Status == QuizAttemptStatus.Submitted)
            return QuizAttemptErrors.AttemptAlreadySubmitted;



        var canMutate = EnsureCanMutate(submittedAtUtc);

        if (canMutate.IsError)
            return canMutate.Errors;



        var scoreResult = CalculateScore(answerKey);

        if (scoreResult.IsError)
            return scoreResult.Errors;



        ScorePercentage = scoreResult.Value;

        Status = QuizAttemptStatus.Submitted;

        SubmittedAtUtc = submittedAtUtc;

        UpdatedAtUtc = DateTimeOffset.UtcNow;



        var gradeResult =
            Grade.CreateFromPercentage(
                ScorePercentage.Value,
                passPercentage);



        if (gradeResult.IsError)
            return gradeResult.Errors;


        Grade = gradeResult.Value;



        AddDomainEvent(
            new QuizSubmittedDomainEvent(
                Id,
                QuizId,
                StudentId,
                ScorePercentage.Value,
                Grade.IsPassed));



        if (Grade.IsPassed)
        {
            AddDomainEvent(
                new QuizPassedDomainEvent(
                    Id,
                    QuizId,
                    StudentId,
                    ScorePercentage.Value));
        }



        return Result.Updated;
    }





    public Result<Updated> Submit(
        Quiz quiz,
        DateTimeOffset submittedAtUtc)
    {
        if (quiz is null)
            return QuizAttemptErrors.InvalidScore;



        if (Status == QuizAttemptStatus.Submitted)
            return QuizAttemptErrors.AttemptAlreadySubmitted;



        var canMutate = EnsureCanMutate(submittedAtUtc);

        if (canMutate.IsError)
            return canMutate.Errors;



        var scoreResult = CalculateScore(quiz);

        if (scoreResult.IsError)
            return scoreResult.Errors;



        ScorePercentage = scoreResult.Value;

        Status = QuizAttemptStatus.Submitted;

        SubmittedAtUtc = submittedAtUtc;

        UpdatedAtUtc = DateTimeOffset.UtcNow;



        var gradeResult =
            Grade.CreateFromPercentage(
                ScorePercentage.Value,
                quiz.PassingPolicy.PassPercentage);



        if (gradeResult.IsError)
            return gradeResult.Errors;


        Grade = gradeResult.Value;



        AddDomainEvent(
            new QuizSubmittedDomainEvent(
                Id,
                QuizId,
                StudentId,
                ScorePercentage.Value,
                Grade.IsPassed));



        if (Grade.IsPassed)
        {
            AddDomainEvent(
                new QuizPassedDomainEvent(
                    Id,
                    QuizId,
                    StudentId,
                    ScorePercentage.Value));
        }



        return Result.Updated;
    }





    public Result<decimal> CalculateScore(
        IReadOnlyDictionary<Guid, HashSet<Guid>> answerKey)
    {
        if (answerKey.Count == 0)
            return QuizAttemptErrors.InvalidScore;



        var correctCount = 0;


        foreach (var item in answerKey)
        {
            var answer =
                _answers.FirstOrDefault(
                    a => a.QuestionId == item.Key);



            if (answer is null)
                continue;



            if (answer.SelectedChoiceId.HasValue &&
                item.Value.Contains(answer.SelectedChoiceId.Value))
            {
                correctCount++;
            }
        }



        var score =
            decimal.Round(
                (correctCount * 100m) / answerKey.Count,
                2);



        return Math.Clamp(score, 0m, 100m);
    }





    public Result<decimal> CalculateScore(Quiz quiz)
    {
        if (quiz is null)
            return QuizAttemptErrors.InvalidScore;



        if (quiz.Questions.Count == 0)
            return QuizAttemptErrors.InvalidScore;



        var totalPoints = quiz.TotalPoints();


        if (totalPoints <= 0)
            return QuizAttemptErrors.InvalidScore;



        decimal earnedPoints = 0;



        foreach (var question in quiz.Questions)
        {
            var answer =
                _answers.FirstOrDefault(
                    x => x.QuestionId == question.Id);



            if (answer is null)
                continue;



            var correct =
                question.Type switch
                {
                    QuestionType.MultipleChoice or QuestionType.TrueFalse =>
                        answer.SelectedChoiceId.HasValue &&
                        question.Choices.Any(
                            c => c.Id == answer.SelectedChoiceId &&
                                 c.IsCorrect),


                    QuestionType.ShortAnswer =>
                        !string.IsNullOrWhiteSpace(answer.TextAnswer) &&
                        !string.IsNullOrWhiteSpace(question.CorrectTextAnswer) &&
                        NormalizeText(answer.TextAnswer)
                        ==
                        NormalizeText(question.CorrectTextAnswer),


                    _ => false
                };



            if (correct)
                earnedPoints += question.Points;
        }



        var percentage =
            decimal.Round(
                (earnedPoints * 100m) / totalPoints,
                2);



        return Math.Clamp(
            percentage,
            0m,
            100m);
    }





    public Result<Updated> MarkTimedOut(
        DateTimeOffset nowUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return Result.Updated;



        if (ExpiresAtUtc.HasValue &&
            nowUtc >= ExpiresAtUtc.Value)
        {
            Status = QuizAttemptStatus.TimedOut;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }



        return Result.Updated;
    }





    public Result<Updated> Cancel(
        DateTimeOffset nowUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return QuizAttemptErrors.AttemptClosed;



        Status = QuizAttemptStatus.Canceled;

        UpdatedAtUtc = nowUtc;


        return Result.Updated;
    }





    private Result<Updated> EnsureCanMutate(
        DateTimeOffset nowUtc)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return QuizAttemptErrors.AttemptClosed;



        if (ExpiresAtUtc.HasValue &&
            nowUtc >= ExpiresAtUtc.Value)
        {
            Status = QuizAttemptStatus.TimedOut;

            UpdatedAtUtc = DateTimeOffset.UtcNow;

            return QuizAttemptErrors.TimeoutReached;
        }



        return Result.Updated;
    }





    private static string NormalizeText(string value)
    {
        return string.Join(
            " ",
            value
                .Trim()
                .ToLowerInvariant()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }
}