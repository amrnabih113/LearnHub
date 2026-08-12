namespace LearnHub.Application.Features.Reviews.Dtos;

public sealed record ReviewSummaryDto(
    double AverageRating,
    int TotalReviews,
    IReadOnlyDictionary<int, int> StarCounts);
