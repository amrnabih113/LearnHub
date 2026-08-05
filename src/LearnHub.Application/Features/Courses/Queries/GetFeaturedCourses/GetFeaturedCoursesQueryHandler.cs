using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Application.Features.Courses.Mappers;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetFeaturedCourses;

public sealed class GetFeaturedCoursesQueryHandler(IAppDbContext context) : IRequestHandler<GetFeaturedCoursesQuery, Result<List<CourseDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<CourseDto>>> Handle(GetFeaturedCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _context.Courses
            .AsNoTracking()
            .Where(x => x.Status == CourseStatus.Published)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        return courses.Select(x => x.ToDto()).ToList();
    }
}