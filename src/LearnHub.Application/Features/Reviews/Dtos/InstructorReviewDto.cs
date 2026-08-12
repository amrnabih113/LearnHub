namespace LearnHub.Application.Features.Reviews.Dtos;

public sealed record InstructorReviewDto(
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
