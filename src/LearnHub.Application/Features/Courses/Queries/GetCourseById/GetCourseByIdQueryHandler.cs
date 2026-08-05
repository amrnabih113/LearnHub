using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Application.Features.Courses.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailsDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseDetailsDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .AsNoTracking()
            // .AsSplitQuery()
            .Include(x => x.Instructor)
            .Include(x => x.Category)
            .Include(x => x.CourseTags)
                .ThenInclude(x => x.Tag)
            .Include(x => x.Sections)
                .ThenInclude(x => x.Lessons)
                    .ThenInclude(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        return course.ToDetailsDto();
    }
}