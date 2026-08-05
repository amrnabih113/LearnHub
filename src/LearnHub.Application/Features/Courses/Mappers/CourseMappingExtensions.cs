using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Classification.Categories;
using LearnHub.Domain.Classification.Tags;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Identity;

namespace LearnHub.Application.Features.Courses.Mappers;

public static class CourseMappingExtensions
{
    public static CourseDto ToDto(this Course course)
        => new(
            course.Id,
            course.Title ?? string.Empty,
            course.Description ?? string.Empty,
            course.InstructorId ?? Guid.Empty,
            course.CategoryId,
            course.ThumbnailUrl,
            course.Level,
            course.Status,
            course.Price.Amount,
            course.Price.Currency,
            course.IsIncludedInSubscription,
            course.RequiredSubscriptionTier,
            course.Language.Code,
            course.Language.Name,
            course.Country);

    public static CourseDetailsDto ToDetailsDto(this Course course)
        => new(
            course.Id,
            course.Title ?? string.Empty,
            course.Description ?? string.Empty,
            course.Instructor?.ToDto(),
            course.Category?.ToDto(),
            course.CourseTags.Select(courseTag => courseTag.Tag.ToDto()).ToArray(),
            course.InstructorId ?? Guid.Empty,
            course.CategoryId,
            course.ThumbnailUrl,
            course.Level,
            course.Status,
            course.Price.Amount,
            course.Price.Currency,
            course.IsIncludedInSubscription,
            course.RequiredSubscriptionTier,
            course.Language.Code,
            course.Language.Name,
            course.Country,
            course.Sections.Select(section => section.ToDto()).ToArray());

    public static CourseContentDto ToContentDto(this Course course)
        => new(
            course.Id,
            course.Sections.Select(section => section.ToDto()).ToArray());

    private static InstructorDto ToDto(this User user)
        => new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Email,
            user.ImageUrl);

    private static CategoryDto ToDto(this Category category)
        => new(
            category.Id,
            category.Name,
            category.Slug,
            category.Description);

    private static TagDto ToDto(this Tag tag)
        => new(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.Description);
}