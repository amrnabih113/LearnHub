namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record SectionDto(
    Guid Id,
    string Title,
    string Description,
    int Order,
    int LessonCount,
    int DurationInMinutes,
    IReadOnlyCollection<LessonDto> Lessons,
    AssessmentContentDto? Assessment = null,
    bool IsLocked = false);