namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record CourseReadinessCheckItemDto(
    string Key,
    bool IsValid,
    string? Message = null);

public sealed record CourseReadinessDto(
    Guid CourseId,
    bool CanPublish,
    IReadOnlyList<CourseReadinessCheckItemDto> Requirements);
