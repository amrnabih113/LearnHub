using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments;

public static class QuizErrors
{
    public static Error CourseIdRequired
    => Error.Validation(code: "DomainError.Quiz.CourseIdRequired",
    description: "Course id is required");

    public static Error TitleRequired
    => Error.Validation(code: "DomainError.Quiz.TitleRequired",
    description: "Quiz title is required");

    public static Error MaxAttemptsInvalid
    => Error.Validation(code: "DomainError.Quiz.MaxAttemptsInvalid",
    description: "Max attempts must be greater than zero");

    public static Error PassPercentageInvalid
    => Error.Validation(code: "DomainError.Quiz.PassPercentageInvalid",
    description: "Pass percentage must be between 1 and 100");

    public static Error TimeLimitInvalid
    => Error.Validation(code: "DomainError.Quiz.TimeLimitInvalid",
    description: "Time limit minutes must be greater than zero when specified");

    public static Error QuestionsRequired
    => Error.Validation(code: "DomainError.Quiz.QuestionsRequired",
    description: "Quiz must contain at least one question");

    public static Error NotDraft
    => Error.Conflict(code: "DomainError.Quiz.NotDraft",
    description: "Only draft quizzes can be modified");

    public static Error NotPublished
    => Error.Conflict(code: "DomainError.Quiz.NotPublished",
    description: "Quiz must be published before attempts can start");

    public static Error QuestionNotFound
    => Error.NotFound(code: "DomainError.Quiz.QuestionNotFound",
    description: "Question was not found");

    public static Error MaxAttemptsExceeded
    => Error.Conflict(code: "DomainError.Quiz.MaxAttemptsExceeded",
    description: "Student exceeded maximum quiz attempts");

    public static Error UnsupportedQuestionTypeForAutoScoring
    => Error.Validation(code: "DomainError.Quiz.UnsupportedQuestionTypeForAutoScoring",
    description: "Quiz contains a question type that cannot be auto-scored");
}
