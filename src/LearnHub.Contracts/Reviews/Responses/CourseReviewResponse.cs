namespace LearnHub.Contracts.Reviews.Responses;

public sealed record CourseReviewResponse(
    Guid Id,
    Guid CourseId,
    Guid StudentId,
    string StudentName,
    string? StudentImageUrl,
    int Rating,
    string Comment,
    string Status,
    DateTimeOffset CreatedAtUtc);
