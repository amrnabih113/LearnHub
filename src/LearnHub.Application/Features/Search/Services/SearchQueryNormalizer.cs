using System.Text.RegularExpressions;

namespace LearnHub.Application.Features.Search.Services;

public sealed partial class SearchQueryNormalizer : ISearchQueryNormalizer
{
    private static readonly Dictionary<string, string[]> SynonymMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "c#", ["c#", "csharp", ".net", "dotnet"] },
        { "csharp", ["c#", "csharp", ".net", "dotnet"] },
        { "dotnet", [".net", "dotnet", "c#", "asp.net"] },
        { ".net", [".net", "dotnet", "c#", "asp.net"] },
        { "js", ["javascript", "js"] },
        { "javascript", ["javascript", "js"] },
        { "ts", ["typescript", "ts"] },
        { "typescript", ["typescript", "ts"] },
        { "py", ["python", "py"] },
        { "python", ["python", "py"] },
        { "sql", ["sql", "mssql", "sql server", "database"] },
        { "react", ["react", "reactjs"] },
        { "node", ["nodejs", "node"] }
    };

    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public string Normalize(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return string.Empty;

        var cleaned = query.Trim().ToLowerInvariant();
        cleaned = WhitespaceRegex.Replace(cleaned, " ");
        return cleaned;
    }

    public IReadOnlyList<string> Tokenize(string query)
    {
        var normalized = Normalize(query);
        if (string.IsNullOrEmpty(normalized))
            return [];

        return normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public IReadOnlyList<string> ExpandSynonyms(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return [];

        var normalized = term.Trim().ToLowerInvariant();
        if (SynonymMap.TryGetValue(normalized, out var synonyms))
        {
            return synonyms;
        }

        return [normalized];
    }
}
