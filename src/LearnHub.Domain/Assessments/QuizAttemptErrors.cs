using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments;

public static class QuizAttemptErrors
{
    public static Error QuizIdRequired
    => Error.Validation(code: "DomainError.QuizAttempt.QuizIdRequired",
    description: "Quiz id is required");

    public static Error CourseIdRequired
    => Error.Validation(code: "DomainError.QuizAttempt.CourseIdRequired",
    description: "Course id is required");

    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.QuizAttempt.StudentIdRequired",
    description: "Student id is required");

    public static Error AttemptNumberInvalid
    => Error.Validation(code: "DomainError.QuizAttempt.AttemptNumberInvalid",
    description: "Attempt number must be greater than zero");

    public static Error QuestionIdRequired
    => Error.Validation(code: "DomainError.QuizAttempt.QuestionIdRequired",
    description: "Question id is required");

    public static Error ChoiceIdRequired
    => Error.Validation(code: "DomainError.QuizAttempt.ChoiceIdRequired",
    description: "Choice id is required");

    public static Error AnswerTextRequired
    => Error.Validation(code: "DomainError.QuizAttempt.AnswerTextRequired",
    description: "Answer text is required");

    public static Error AttemptClosed
    => Error.Conflict(code: "DomainError.QuizAttempt.AttemptClosed",
    description: "Attempt cannot be modified after submission, timeout, or cancellation");

    public static Error AttemptAlreadySubmitted
    => Error.Conflict(code: "DomainError.QuizAttempt.AttemptAlreadySubmitted",
    description: "Attempt is already submitted");

    public static Error TimeoutReached
    => Error.Conflict(code: "DomainError.QuizAttempt.TimeoutReached",
    description: "Quiz attempt timeout reached");

    public static Error InvalidScore
    => Error.Validation(code: "DomainError.QuizAttempt.InvalidScore",
    description: "Score must be between 0 and 100");

    public static Error AttemptNotFound
    => Error.NotFound(code: "DomainError.QuizAttempt.AttemptNotFound",
    description: "Quiz attempt was not found");

    public static Error StudentMismatch
    => Error.Forbidden(code: "DomainError.QuizAttempt.StudentMismatch",
    description: "Quiz attempt belongs to another student");

    public static Error OptionRequired
    => Error.Validation(code: "DomainError.QuizAttempt.OptionRequired",
    description: "Choice ID or text answer is required");
}
