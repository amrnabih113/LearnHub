using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.DeleteLearningPath;

public sealed class DeleteLearningPathCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteLearningPathCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(
        DeleteLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var archiveResult = path.Archive();
        if (archiveResult.IsError)
        {
            return archiveResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
