using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Queries.GetLearningPaths;

public sealed class GetLearningPathsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLearningPathsQuery, Result<PagedResult<LearningPathDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<LearningPathDto>>> Handle(
        GetLearningPathsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.LearningPaths
            .Include(lp => lp.Owner)
            .Include(lp => lp.Courses)
            .AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(lp => lp.Status == request.Status.Value);
        }

        if (request.Level.HasValue)
        {
            query = query.Where(lp => lp.Level == request.Level.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(lp => lp.Title.ToLower().Contains(search) ||
                                     lp.Description.ToLower().Contains(search) ||
                                     lp.ShortDescription.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(lp => lp.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(lp => new LearningPathDto(
                lp.Id,
                lp.Title,
                lp.Slug,
                lp.Description,
                lp.ShortDescription,
                lp.ThumbnailUrl,
                lp.Level,
                lp.Status,
                lp.OwnerId,
                lp.Owner != null ? (lp.Owner.FirstName + " " + lp.Owner.LastName) : null,
                lp.Courses.Count,
                lp.CreatedAtUtc,
                lp.PublishedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<LearningPathDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
