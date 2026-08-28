using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.UpdateLearningPath;

public sealed class UpdateLearningPathCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateLearningPathCommand, Result<LearningPathDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<LearningPathDto>> Handle(
        UpdateLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses)
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var updateResult = path.Update(
            request.Title,
            request.Slug,
            request.Description,
            request.ShortDescription,
            request.ThumbnailUrl,
            request.Level);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        string? ownerName = null;
        if (path.OwnerId.HasValue)
        {
            ownerName = await _context.Users
                .Where(u => u.Id == path.OwnerId.Value)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new LearningPathDto(
            path.Id,
            path.Title,
            path.Slug,
            path.Description,
            path.ShortDescription,
            path.ThumbnailUrl,
            path.Level,
            path.Status,
            path.OwnerId,
            ownerName,
            path.Courses.Count,
            path.CreatedAtUtc,
            path.PublishedAtUtc);
    }
}
