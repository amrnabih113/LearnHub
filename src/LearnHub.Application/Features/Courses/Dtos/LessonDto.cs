namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record LessonDto(
    Guid Id,
    string Title,
    string Description,
    string VideoUrl,
    bool IsPreview,
    string Content,
    int DurationInMinutes,
    int Order,
    IReadOnlyCollection<ResourceDto> Resources);