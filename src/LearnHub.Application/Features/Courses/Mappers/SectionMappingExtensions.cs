using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Courses.Sections;

namespace LearnHub.Application.Features.Courses.Mappers;

public static class SectionMappingExtensions
{
    public static SectionDto ToDto(this Section section)
        => new(
            section.Id,
            section.Title,
            section.Description,
            section.Order,
            section.LessonCount,
            section.DurationInMinutes,
            section.Lessons.Select(lesson => lesson.ToDto()).ToArray());
}