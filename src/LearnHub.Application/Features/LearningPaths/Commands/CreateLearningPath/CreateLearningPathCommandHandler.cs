using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Commands.CreateLearningPath;

public sealed class CreateLearningPathCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateLearningPathCommand, Result<LearningPathDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<LearningPathDto>> Handle(
        CreateLearningPathCommand request,
        CancellationToken cancellationToken)
    {
        var createResult = LearningPath.Create(
            Guid.NewGuid(),
            request.Title,
            request.Slug,
            request.Description,
            request.ShortDescription,
            request.ThumbnailUrl,
            request.Level,
            request.OwnerId);

        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        var path = createResult.Value;

        // Check Slug uniqueness
        var existingSlug = await _context.LearningPaths
            .AnyAsync(lp => lp.Slug == path.Slug, cancellationToken);

        if (existingSlug)
        {
            return Error.Conflict("LearningPath.SlugExists", "A learning path with this slug already exists.");
        }

        _context.LearningPaths.Add(path);
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
            0,
            path.CreatedAtUtc,
            path.PublishedAtUtc);
    }
}
