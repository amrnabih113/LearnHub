using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Reviews.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Reviews.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Reviews.Queries.GetCourseReviewSummary;

public sealed class GetCourseReviewSummaryQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCourseReviewSummaryQuery, Result<ReviewSummaryDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<ReviewSummaryDto>> Handle(
        GetCourseReviewSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var ratings = await _context.CourseReviews
            .AsNoTracking()
            .Where(r => r.CourseId == request.CourseId && r.Status == ReviewStatus.Published)
            .Select(r => r.Rating.Value)
            .ToListAsync(cancellationToken);

        if (ratings.Count == 0)
        {
            var emptyStarCounts = new Dictionary<int, int> { [1] = 0, [2] = 0, [3] = 0, [4] = 0, [5] = 0 };
            return new ReviewSummaryDto(AverageRating: 0, TotalReviews: 0, StarCounts: emptyStarCounts);
        }

        var totalReviews = ratings.Count;
        var averageRating = Math.Round(ratings.Average(), 1);

        var starCounts = new Dictionary<int, int>
        {
            [1] = ratings.Count(r => r == 1),
            [2] = ratings.Count(r => r == 2),
            [3] = ratings.Count(r => r == 3),
            [4] = ratings.Count(r => r == 4),
            [5] = ratings.Count(r => r == 5)
        };

        return new ReviewSummaryDto(
            AverageRating: averageRating,
            TotalReviews: totalReviews,
            StarCounts: starCounts);
    }
}
