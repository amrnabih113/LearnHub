namespace LearnHub.Contracts.Reviews.Responses;

public sealed record InstructorReviewResponse(
    Guid Id,
    Guid InstructorId,
    Guid StudentId,
    string StudentName,
    string? StudentImageUrl,
    Guid? CourseId,
    int Rating,
    string Comment,
    string Status,
    DateTimeOffset CreatedAtUtc);
