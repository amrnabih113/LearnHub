using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Courses.Sections.Lessons.Resources;

namespace LearnHub.Application.Features.Courses.Mappers;

public static class ResourceMappingExtensions
{
    public static ResourceDto ToDto(this Resource resource)
        => new(
            resource.Id,
            resource.Name,
            resource.Url,
            resource.Type,
            resource.SizeInBytes);
}