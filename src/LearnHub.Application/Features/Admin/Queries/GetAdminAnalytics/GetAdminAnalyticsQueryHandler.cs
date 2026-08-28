using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetAdminAnalytics;

public sealed class GetAdminAnalyticsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAdminAnalyticsQuery, Result<AdminAnalyticsDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<AdminAnalyticsDto>> Handle(
        GetAdminAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        int months = Math.Clamp(request.MonthsBack, 1, 24);
        var now = DateTimeOffset.UtcNow;
        var startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(-months + 1);

        var usersRaw = await _context.Users
            .AsNoTracking()
            .Where(u => u.CreatedAtUtc >= startDate)
            .Select(u => u.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var enrollmentsRaw = await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.CreatedAtUtc >= startDate)
            .Select(e => new { e.CreatedAtUtc, e.Status, e.CompletedAtUtc })
            .ToListAsync(cancellationToken);

        var coursesRaw = await _context.Courses
            .AsNoTracking()
            .Where(c => c.CreatedAtUtc >= startDate)
            .Select(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var paymentsRaw = await _context.Payments
            .AsNoTracking()
            .Where(p => p.Status == Domain.Common.Enums.PaymentStatus.Succeeded && p.CreatedAtUtc >= startDate)
            .Select(p => new { p.CreatedAtUtc, Amount = p.Amount.Amount })
            .ToListAsync(cancellationToken);

        var subscriptionsRaw = await _context.Subscriptions
            .AsNoTracking()
            .Where(s => s.CreatedAtUtc >= startDate)
            .Select(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var userGrowth = new List<MonthlyGrowthTrendDto>();
        var enrollmentGrowth = new List<MonthlyGrowthTrendDto>();
        var courseGrowth = new List<MonthlyGrowthTrendDto>();
        var revenueTrends = new List<MonthlyRevenueTrendDto>();
        var subscriptionGrowth = new List<MonthlyGrowthTrendDto>();
        var completionTrends = new List<MonthlyGrowthTrendDto>();

        for (int i = months - 1; i >= 0; i--)
        {
            var targetMonth = now.AddMonths(-i);
            int m = targetMonth.Month;
            int y = targetMonth.Year;
            string monthName = targetMonth.ToString("MMM");

            int uCount = usersRaw.Count(u => u.Month == m && u.Year == y);
            int eCount = enrollmentsRaw.Count(e => e.CreatedAtUtc.Month == m && e.CreatedAtUtc.Year == y);
            int cCount = coursesRaw.Count(c => c.Month == m && c.Year == y);
            decimal rev = paymentsRaw.Where(p => p.CreatedAtUtc.Month == m && p.CreatedAtUtc.Year == y).Sum(p => p.Amount);
            int subCount = subscriptionsRaw.Count(s => s.Month == m && s.Year == y);
            int compCount = enrollmentsRaw.Count(e => e.CompletedAtUtc.HasValue && e.CompletedAtUtc.Value.Month == m && e.CompletedAtUtc.Value.Year == y);

            userGrowth.Add(new MonthlyGrowthTrendDto(monthName, y, uCount));
            enrollmentGrowth.Add(new MonthlyGrowthTrendDto(monthName, y, eCount));
            courseGrowth.Add(new MonthlyGrowthTrendDto(monthName, y, cCount));
            revenueTrends.Add(new MonthlyRevenueTrendDto(monthName, y, rev, "USD"));
            subscriptionGrowth.Add(new MonthlyGrowthTrendDto(monthName, y, subCount));
            completionTrends.Add(new MonthlyGrowthTrendDto(monthName, y, compCount));
        }

        return new AdminAnalyticsDto(
            Timeframe: $"Last {months} Months",
            UserGrowth: userGrowth,
            EnrollmentGrowth: enrollmentGrowth,
            CourseGrowth: courseGrowth,
            RevenueTrends: revenueTrends,
            SubscriptionGrowth: subscriptionGrowth,
            CompletionTrends: completionTrends);
    }
}
