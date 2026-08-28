using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.PublishCourse;

public sealed record PublishCourseCommand(Guid CourseId, Guid InstructorId)
    : IRequest<Result<Updated>>;

public sealed class PublishCourseCommandHandler(IAppDbContext context)
    : IRequestHandler<PublishCourseCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        PublishCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course was not found.");
        }

        if (course.InstructorId != request.InstructorId)
        {
            return Error.Forbidden("Course.Forbidden", "Instructor does not own this course.");
        }

        var publishResult = course.Publish();
        if (publishResult.IsError)
        {
            return publishResult.Errors;
        }

        // Publish all existing sections and lessons on initial full course publish
        foreach (var section in course.Sections)
        {
            section.Publish();
            foreach (var lesson in section.Lessons)
            {
                lesson.Publish();
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
