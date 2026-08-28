using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.PublishLearningPath;

public sealed class PublishLearningPathCommandHandler(IAppDbContext context)
    : IRequestHandler<PublishLearningPathCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        PublishLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses)
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var publishResult = path.Publish();
        if (publishResult.IsError)
        {
            return publishResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
