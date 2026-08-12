using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetReviewsAdmin;

public sealed class GetReviewsAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetReviewsAdminQuery, Result<PagedResult<CourseReviewDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<CourseReviewDto>>> Handle(
        GetReviewsAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.CourseReviews
            .Include(r => r.Student)
            .AsNoTracking();

        if (request.CourseId.HasValue)
        {
            query = query.Where(r => r.CourseId == request.CourseId.Value);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(r => r.StudentId == request.StudentId.Value);
        }

        if (request.Rating.HasValue)
        {
            query = query.Where(r => r.Rating.Value == request.Rating.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<ReviewStatus>(request.Status, true, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new CourseReviewDto(
                r.Id,
                r.CourseId,
                r.StudentId,
                r.Student != null ? r.Student.FullName : string.Empty,
                r.Student != null ? r.Student.ImageUrl : null,
                r.Rating.Value,
                r.Comment,
                r.Status.ToString(),
                r.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<CourseReviewDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
