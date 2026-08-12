using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetTagsAdmin;

public sealed class GetTagsAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetTagsAdminQuery, Result<PagedResult<TagAdminDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<TagAdminDto>>> Handle(
        GetTagsAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Tags.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(search) || t.Slug.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TagAdminDto(
                t.Id,
                t.Name,
                t.Slug,
                t.Description,
                t.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<TagAdminDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
