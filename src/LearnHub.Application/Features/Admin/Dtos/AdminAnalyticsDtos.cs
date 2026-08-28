namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record AdminAnalyticsDto(
    string Timeframe,
    IReadOnlyList<MonthlyGrowthTrendDto> UserGrowth,
    IReadOnlyList<MonthlyGrowthTrendDto> EnrollmentGrowth,
    IReadOnlyList<MonthlyGrowthTrendDto> CourseGrowth,
    IReadOnlyList<MonthlyRevenueTrendDto> RevenueTrends,
    IReadOnlyList<MonthlyGrowthTrendDto> SubscriptionGrowth,
    IReadOnlyList<MonthlyGrowthTrendDto> CompletionTrends);

public sealed record MonthlyGrowthTrendDto(
    string Month,
    int Year,
    int Count);

public sealed record MonthlyRevenueTrendDto(
    string Month,
    int Year,
    decimal Revenue,
    string Currency);
