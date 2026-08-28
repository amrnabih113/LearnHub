using LearnHub.Application.Features.Search.Models;

namespace LearnHub.Application.Features.Search.Services;

public interface ISearchRankingService
{
    SearchCandidate CalculateFinalScore(SearchCandidate candidate);
}

public sealed class SearchRankingService : ISearchRankingService
{
    // Configurable weights
    private const double ExactWeight = 0.35;
    private const double FullTextWeight = 0.25;
    private const double FuzzyWeight = 0.10;
    private const double SynonymWeight = 0.10;
    private const double SemanticWeight = 0.10;
    private const double RatingWeight = 0.05;
    private const double PopularityWeight = 0.05;

    public SearchCandidate CalculateFinalScore(SearchCandidate candidate)
    {
        double finalScore =
            (candidate.ExactScore * ExactWeight) +
            (candidate.FullTextScore * FullTextWeight) +
            (candidate.FuzzyScore * FuzzyWeight) +
            (candidate.SynonymScore * SynonymWeight) +
            (candidate.SemanticScore * SemanticWeight) +
            (candidate.RatingScore * RatingWeight) +
            (candidate.PopularityScore * PopularityWeight);

        return candidate with { FinalScore = Math.Round(finalScore, 4) };
    }
}
