namespace LearnHub.Application.Features.Reviews.Dtos;

public sealed record CourseReviewDto(
    Guid Id,
    Guid CourseId,
    Guid StudentId,
    string StudentName,
    string? StudentImageUrl,
    int Rating,
    string Comment,
    string Status,
    DateTimeOffset CreatedAtUtc);
