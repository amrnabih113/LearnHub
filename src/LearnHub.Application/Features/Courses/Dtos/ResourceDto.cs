using LearnHub.Domain.Courses.Sections.Lessons.Resources;

namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record ResourceDto(
    Guid Id,
    string Name,
    string Url,
    ResourceType Type,
    long SizeInBytes);