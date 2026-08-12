namespace LearnHub.Contracts.Reviews.Responses;

public sealed record ReviewSummaryResponse(
    double AverageRating,
    int TotalReviews,
    IReadOnlyDictionary<int, int> StarCounts);
