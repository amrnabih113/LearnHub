namespace LearnHub.Application.Features.Search.Services;

public interface ISynonymProvider
{
    IReadOnlyList<string> GetSynonyms(string term);
}

public sealed class SynonymProvider : ISynonymProvider
{
    private static readonly Dictionary<string, string[]> Synonyms = new(StringComparer.OrdinalIgnoreCase)
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

    public IReadOnlyList<string> GetSynonyms(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return [];

        var key = term.Trim().ToLowerInvariant();
        return Synonyms.TryGetValue(key, out var list) ? list : [key];
    }
}
