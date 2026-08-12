using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetOrdersAdmin;

public sealed class GetOrdersAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetOrdersAdminQuery, Result<PagedResult<OrderAdminSummaryDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<OrderAdminSummaryDto>>> Handle(
        GetOrdersAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var statusEnum))
        {
            query = query.Where(o => o.Status == statusEnum);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAtUtc >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.CreatedAtUtc <= request.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            if (Guid.TryParse(search, out var searchGuid))
            {
                query = query.Where(o => o.Id == searchGuid || o.StudentId == searchGuid);
            }
            else
            {
                var matchingStudentIds = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Email.ToLower().Contains(search) || u.FirstName.ToLower().Contains(search) || u.LastName.ToLower().Contains(search))
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);

                query = query.Where(o => matchingStudentIds.Contains(o.StudentId));
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var ordersRaw = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var studentIds = ordersRaw.Select(o => o.StudentId).Distinct().ToList();
        var students = await _context.Users
            .AsNoTracking()
            .Where(u => studentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var items = ordersRaw.Select(o =>
        {
            students.TryGetValue(o.StudentId, out var student);
            return new OrderAdminSummaryDto(
                o.Id,
                o.StudentId,
                student?.FullName ?? string.Empty,
                student?.Email ?? string.Empty,
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.Status.ToString(),
                o.Items.Count,
                o.CreatedAtUtc);
        }).ToList();

        return new PagedResult<OrderAdminSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
