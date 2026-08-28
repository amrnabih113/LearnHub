using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.AddCourseToLearningPath;

public sealed class AddCourseToLearningPathCommandHandler(IAppDbContext context)
    : IRequestHandler<AddCourseToLearningPathCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        AddCourseToLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses)
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

        if (!courseExists)
        {
            return Error.NotFound("Course.NotFound", "The specified course was not found.");
        }

        var addResult = path.AddCourse(request.CourseId, request.TargetOrder, request.IsRequired);
        if (addResult.IsError)
        {
            return addResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
