using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.RemoveCourseTag;

public sealed class RemoveCourseTagCommandHandler(IAppDbContext context) : IRequestHandler<RemoveCourseTagCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(RemoveCourseTagCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.Include(x => x.CourseTags).FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        var result = course.RemoveTag(request.TagId);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}