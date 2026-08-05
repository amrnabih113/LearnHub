using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Courses.Sections.Lessons;

namespace LearnHub.Application.Features.Courses.Mappers;

public static class LessonMappingExtensions
{
    public static LessonDto ToDto(this Lesson lesson)
        => new(
            lesson.Id,
            lesson.Title ?? string.Empty,
            lesson.Description ?? string.Empty,
            lesson.VideoUrl,
            lesson.IsPreview,
            lesson.Content ?? string.Empty,
            lesson.DurationInMinutes,
            lesson.Order,
            lesson.Resources.Select(resource => resource.ToDto()).ToArray());
}