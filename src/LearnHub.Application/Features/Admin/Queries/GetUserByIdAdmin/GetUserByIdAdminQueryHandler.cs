using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetUserByIdAdmin;

public sealed class GetUserByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetUserByIdAdminQuery, Result<UserAdminDetailDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<UserAdminDetailDto>> Handle(
        GetUserByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var enrollmentsRaw = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == request.Id)
            .OrderByDescending(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var courseIds = enrollmentsRaw.Select(e => e.CourseId).Distinct().ToList();
        var courseTitles = await _context.Courses
            .AsNoTracking()
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Title, cancellationToken);

        var enrollments = enrollmentsRaw.Select(e => new UserEnrollmentDto(
            e.Id,
            e.CourseId,
            courseTitles.TryGetValue(e.CourseId, out var title) ? title : string.Empty,
            e.Status.ToString(),
            e.ProgressPercentage,
            e.CreatedAtUtc)).ToList();

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.StudentId == request.Id)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new UserOrderDto(
                o.Id,
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.Status.ToString(),
                o.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        var subscriptions = await _context.Subscriptions
            .Include(s => s.Plan)
            .AsNoTracking()
            .Where(s => s.StudentId == request.Id)
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => new UserSubscriptionDto(
                s.Id,
                s.Plan != null ? s.Plan.Tier.ToString() : s.Tier.ToString(),
                s.Status.ToString(),
                s.StartedAtUtc,
                s.ExpiresAtUtc))
            .ToListAsync(cancellationToken);

        var reviews = await _context.CourseReviews
            .AsNoTracking()
            .Where(r => r.StudentId == request.Id)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new UserReviewDto(
                r.Id,
                r.CourseId,
                r.Rating.Value,
                r.Comment,
                r.Status.ToString(),
                r.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new UserAdminDetailDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.ImageUrl,
            user.Roles.Select(r => r.Role.ToString()).ToList(),
            user.IsEmailVerified,
            user.CreatedAtUtc,
            enrollments,
            orders,
            subscriptions,
            reviews);
    }
}
