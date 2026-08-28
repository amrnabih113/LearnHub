using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.RemoveCourseFromLearningPath;

public sealed class RemoveCourseFromLearningPathCommandHandler(IAppDbContext context)
    : IRequestHandler<RemoveCourseFromLearningPathCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        RemoveCourseFromLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses)
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var removeResult = path.RemoveCourse(request.CourseId);
        if (removeResult.IsError)
        {
            return removeResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
