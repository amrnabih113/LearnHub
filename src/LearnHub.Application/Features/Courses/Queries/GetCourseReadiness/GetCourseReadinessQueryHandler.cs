using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseReadiness;

public sealed record GetCourseReadinessQuery(Guid CourseId)
    : IRequest<Result<CourseReadinessDto>>;

public sealed class GetCourseReadinessQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCourseReadinessQuery, Result<CourseReadinessDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseReadinessDto>> Handle(
        GetCourseReadinessQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course was not found.");
        }

        var checks = new List<CourseReadinessCheckItemDto>
        {
            new("title", !string.IsNullOrWhiteSpace(course.Title), !string.IsNullOrWhiteSpace(course.Title) ? "Title is set" : "Title is required"),
            new("description", !string.IsNullOrWhiteSpace(course.Description), !string.IsNullOrWhiteSpace(course.Description) ? "Description is set" : "Description is required"),
            new("thumbnail", !string.IsNullOrWhiteSpace(course.ThumbnailUrl), !string.IsNullOrWhiteSpace(course.ThumbnailUrl) ? "Thumbnail is set" : "Thumbnail is required"),
            new("category", course.CategoryId != Guid.Empty, course.CategoryId != Guid.Empty ? "Category is assigned" : "Category is required"),
            new("price", course.Price != null, course.Price != null ? "Price is set" : "Price is required"),
            new("sections", course.Sections.Any(), course.Sections.Any() ? $"{course.Sections.Count()} sections created" : "Course must contain at least one section"),
            new("lessons", course.Sections.Any(s => s.Lessons.Any()), course.Sections.Any(s => s.Lessons.Any()) ? "Lessons created" : "At least one section must contain lessons"),
            new("videos", course.Sections.SelectMany(s => s.Lessons).Any() && course.Sections.SelectMany(s => s.Lessons).All(l => !string.IsNullOrWhiteSpace(l.VideoUrl)),
                course.Sections.SelectMany(s => s.Lessons).All(l => !string.IsNullOrWhiteSpace(l.VideoUrl)) ? "All lesson videos uploaded" : "Some lesson videos are missing or not uploaded")
        };

        bool canPublish = checks.All(c => c.IsValid);

        return new CourseReadinessDto(course.Id, canPublish, checks);
    }
}
