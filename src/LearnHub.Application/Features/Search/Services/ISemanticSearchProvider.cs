namespace LearnHub.Application.Features.Search.Services;

public interface ISemanticSearchProvider
{
    Task<IDictionary<Guid, double>> GetSemanticScoresAsync(
        string query,
        IReadOnlyList<Guid> candidateCourseIds,
        CancellationToken cancellationToken = default);
}

public sealed class NullSemanticSearchProvider : ISemanticSearchProvider
{
    public Task<IDictionary<Guid, double>> GetSemanticScoresAsync(
        string query,
        IReadOnlyList<Guid> candidateCourseIds,
        CancellationToken cancellationToken = default)
    {
        IDictionary<Guid, double> scores = new Dictionary<Guid, double>();
        foreach (var id in candidateCourseIds)
        {
            scores[id] = 0.0;
        }
        return Task.FromResult(scores);
    }
}
