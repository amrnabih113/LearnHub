namespace LearnHub.Application.Features.Search.Services;

public interface IFuzzyMatcher
{
    double CalculateSimilarity(string source, string target);
    int LevenshteinDistance(string source, string target);
}

public sealed class FuzzyMatcher : IFuzzyMatcher
{
    public double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0.0;

        var s = source.Trim().ToLowerInvariant();
        var t = target.Trim().ToLowerInvariant();

        if (s == t) return 1.0;

        int distance = LevenshteinDistance(s, t);
        int maxLength = Math.Max(s.Length, t.Length);
        return maxLength == 0 ? 1.0 : 1.0 - ((double)distance / maxLength);
    }

    public int LevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
        if (string.IsNullOrEmpty(target)) return source.Length;

        int n = source.Length;
        int m = target.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) { }
        for (int j = 0; j <= m; d[0, j] = j++) { }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }
}
