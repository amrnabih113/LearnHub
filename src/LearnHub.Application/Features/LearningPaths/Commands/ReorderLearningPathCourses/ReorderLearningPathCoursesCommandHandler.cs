using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.ReorderLearningPathCourses;

public sealed class ReorderLearningPathCoursesCommandHandler(IAppDbContext context)
    : IRequestHandler<ReorderLearningPathCoursesCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        ReorderLearningPathCoursesCommand request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses)
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var reorderResult = path.ReorderCourses(request.OrderedCourseIds);
        if (reorderResult.IsError)
        {
            return reorderResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
