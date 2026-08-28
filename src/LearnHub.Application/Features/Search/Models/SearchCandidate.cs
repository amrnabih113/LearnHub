namespace LearnHub.Application.Features.Search.Models;

public sealed record SearchCandidate(
    Guid CourseId,
    double ExactScore = 0.0,
    double FullTextScore = 0.0,
    double FuzzyScore = 0.0,
    double SynonymScore = 0.0,
    double SemanticScore = 0.0,
    double RatingScore = 0.0,
    double PopularityScore = 0.0)
{
    public double FinalScore { get; init; }
}
