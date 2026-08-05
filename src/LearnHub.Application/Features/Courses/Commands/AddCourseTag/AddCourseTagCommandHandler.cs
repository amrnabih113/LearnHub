using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Commands.AddCourseTag;

public sealed class AddCourseTagCommandHandler(IAppDbContext context) : IRequestHandler<AddCourseTagCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(AddCourseTagCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.Include(x => x.CourseTags).FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        var tagExists = await _context.Tags.AnyAsync(x => x.Id == request.TagId, cancellationToken);
        if (!tagExists)
        {
            return Error.NotFound("ApplicationError.Course.TagNotFound", "Tag not found.");
        }

        var result = course.AddTag(request.TagId);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}