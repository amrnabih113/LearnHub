using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Application.Features.Courses.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseContent;

public sealed class GetCourseContentQueryHandler(IAppDbContext context) : IRequestHandler<GetCourseContentQuery, Result<CourseContentDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseContentDto>> Handle(GetCourseContentQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .AsNoTracking()
            // .AsSplitQuery()
            .Include(x => x.Sections)
                .ThenInclude(x => x.Lessons)
                    .ThenInclude(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        return course.ToContentDto();
    }
}