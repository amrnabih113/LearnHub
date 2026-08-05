namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record CourseContentDto(
    Guid CourseId,
    IReadOnlyCollection<SectionDto> Sections);