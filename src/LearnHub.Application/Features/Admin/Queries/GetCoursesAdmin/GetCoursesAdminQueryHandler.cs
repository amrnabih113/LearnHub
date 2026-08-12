using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetCoursesAdmin;

public sealed class GetCoursesAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCoursesAdminQuery, Result<PagedResult<CourseAdminSummaryDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<CourseAdminSummaryDto>>> Handle(
        GetCoursesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(search) || c.Description.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<CourseStatus>(request.Status, true, out var statusEnum))
        {
            query = query.Where(c => c.Status == statusEnum);
        }

        if (request.InstructorId.HasValue)
        {
            query = query.Where(c => c.InstructorId == request.InstructorId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == request.CategoryId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var coursesRaw = await query
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var instructorIds = coursesRaw.Where(c => c.InstructorId.HasValue).Select(c => c.InstructorId!.Value).Distinct().ToList();
        var instructors = await _context.Users
            .AsNoTracking()
            .Where(u => instructorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var categoryIds = coursesRaw.Select(c => c.CategoryId).Distinct().ToList();
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(cat => categoryIds.Contains(cat.Id))
            .ToDictionaryAsync(cat => cat.Id, cat => cat.Name, cancellationToken);

        var items = coursesRaw.Select(c => new CourseAdminSummaryDto(
            c.Id,
            c.Title,
            c.Status.ToString(),
            c.Price.Amount,
            c.Price.Currency,
            c.InstructorId,
            c.InstructorId.HasValue && instructors.TryGetValue(c.InstructorId.Value, out var insName) ? insName : string.Empty,
            c.CategoryId,
            categories.TryGetValue(c.CategoryId, out var catName) ? catName : string.Empty,
            c.IsIncludedInSubscription,
            c.CreatedAtUtc)).ToList();

        return new PagedResult<CourseAdminSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
