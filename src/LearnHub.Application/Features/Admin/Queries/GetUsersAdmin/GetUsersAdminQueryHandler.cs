using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetUsersAdmin;

public sealed class GetUsersAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetUsersAdminQuery, Result<PagedResult<UserAdminSummaryDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<UserAdminSummaryDto>>> Handle(
        GetUsersAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.Roles)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(u => u.Email.ToLower().Contains(search)
                                  || u.FirstName.ToLower().Contains(search)
                                  || u.LastName.ToLower().Contains(search));
        }

        if (request.IsEmailVerified.HasValue)
        {
            query = query.Where(u => u.IsEmailVerified == request.IsEmailVerified.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Role) && Enum.TryParse<Role>(request.Role, true, out var roleEnum))
        {
            query = query.Where(u => u.Roles.Any(r => r.Role == roleEnum));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var users = await query
            .OrderByDescending(u => u.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserAdminSummaryDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.PhoneNumber,
                u.ImageUrl,
                u.Roles.Select(r => r.Role.ToString()).ToList(),
                u.IsEmailVerified,
                u.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserAdminSummaryDto>
        {
            Items = users,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
