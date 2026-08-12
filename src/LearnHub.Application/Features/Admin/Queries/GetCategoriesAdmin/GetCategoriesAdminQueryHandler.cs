using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetCategoriesAdmin;

public sealed class GetCategoriesAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCategoriesAdminQuery, Result<PagedResult<CategoryAdminDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<CategoryAdminDto>>> Handle(
        GetCategoriesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(search) || c.Slug.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var categoriesRaw = await query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var parentIds = categoriesRaw
            .Where(c => c.ParentCategoryId.HasValue)
            .Select(c => c.ParentCategoryId!.Value)
            .Distinct()
            .ToList();

        var parentNames = await _context.Categories
            .AsNoTracking()
            .Where(c => parentIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var items = categoriesRaw.Select(c => new CategoryAdminDto(
            c.Id,
            c.Name,
            c.Slug,
            c.Description,
            c.ParentCategoryId,
            c.ParentCategoryId.HasValue && parentNames.TryGetValue(c.ParentCategoryId.Value, out var parentName) ? parentName : null,
            c.CreatedAtUtc)).ToList();

        return new PagedResult<CategoryAdminDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
